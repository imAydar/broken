using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BrokenCode.Cfg;
using BrokenCode.Etc;
using BrokenCode.Helpers;
using BrokenCode.Interfaces;
using BrokenCode.Mappings;
using BrokenCode.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using log4net;

namespace BrokenCode
{
    /// <summary>
    /// Fixed broken service.
    /// </summary>
    public class BrokenService
    {
        /// <summary>
        /// Counter for attempts logging.
        /// </summary>
        private int _counter = 0;

        /// <summary>
        /// <see cref="RetrySettings"/>
        /// </summary>
        private readonly RetrySettings _retrySettings;

        /// <summary>
        /// <see cref="LicenseSettings"/>
        /// </summary>
        private readonly LicenseSettings _licenseSettings;

        /// <summary>
        /// <see cref="ILicenseServiceProvider"/>
        /// </summary>
        private readonly ILicenseServiceProvider _licenseServiceProvider;

        /// <summary>
        /// <see cref="IUserRepository"/>
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// <see cref="ILog"/>
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrokenService));

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="licenseServiceProvider"><see cref="ILicenseServiceProvider"/></param>
        /// <param name="userRepository"><see cref="IUserRepository"/></param>
        /// <param name="retrySettings"><see cref="RetrySettings"/></param>
        /// <param name="licenseSettings"><see cref="LicenseSettings"/></param>
        public BrokenService(ILicenseServiceProvider licenseServiceProvider,
            IUserRepository userRepository, 
            IOptions<RetrySettings> retrySettings,
            IOptions<LicenseSettings> licenseSettings)
        {
            _licenseServiceProvider = licenseServiceProvider;
            _userRepository = userRepository;
            
            _retrySettings = retrySettings.Value;
            _licenseSettings = licenseSettings.Value;
        }

        /// <summary>
        /// Tries to get report asynchronously using Retry helper.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetReportAsync(GetReportRequest request)
        {
            //Original code was using Monitor, but i can't see any reason to use it.
            //If i had to use it(in case of multithreading), i'd preferably use SemaphoreSlim,
            //because Monitor won't work correctly with async methods.
            //Also for increment i'd use the atomic operation Interlocked.Increment(ref _counter).
            //Also i'd not implement my own Retry if it'd be enough to use the Polly library, but
            //there's some additional handlers need to be implemented.
            return await Retry<IActionResult>
                .Attempts(_retrySettings.AttemptsNumber)
                .Interval(_retrySettings.Interval)
                .Invoke(async () => await GetReportAsyncInner(request),
                    OnGetReportFailed,
                    OnAttemptsExceed);
        }

        /// <summary>
        /// Returns OkObjectResult with result or throws exception
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<IActionResult> GetReportAsyncInner(GetReportRequest request)
        {
            var filteredUsers = await _userRepository.GetUsersAsync(request.ToGetUsersAsyncParam());
            int userTotalCount = filteredUsers?.Count() ?? 0;
            filteredUsers = filteredUsers.Paginate(request.PageSize, request.PageNumber);

            if(filteredUsers?.Any() != true)
            {
                throw new Exception("No users were found");
            }

            var guidsToLicences = await GetGuidToLicencesDictionaryAsync(request, filteredUsers);

            var usersStatistic = filteredUsers.Select(u => u.ToStatistics(guidsToLicences));
            
            //Might be better to return an actual class instead of anonymous type, for example we
            //could see schemas in Swagger.
            return new OkObjectResult(new
            {
                TotaCount = userTotalCount,
                Data = usersStatistic
            });
        }

        //The code bellow is all about licences and i'd move it to LicenceService, but we already
        //have ILicenseService interface and i assume that implementation of it is consider, so
        //considering the task was not to change anything other brokenService class I left it as it
        //is.
        #region licenseLogic
        //Also i almost never use regions unless other code is written with it(in monolithic app for
        //example). I think code with decent architecture don't need to have regions.
        private async Task<Dictionary<Guid, LicenseInfo>> GetGuidToLicencesDictionaryAsync(GetReportRequest request,
            IEnumerable<User> filteredUsers)
        {
            var emails = filteredUsers
                .Select(u => u.UserEmail)
                .ToList();

            using var licenseService = GetLicenseService();

            if (licenseService == null)
            {
                throw new Exception("No licenseService were given");
            }

            Log.Info($"Total licenses for domain '{request.DomainId}': " +
                     $"{licenseService.GetLicensedUserCountAsync(request.DomainId)}");

            var licenses = await GetLicensesAsync(request, licenseService, emails);

            if (licenses == null)
            {
                throw new Exception("No licenses info were given");
            }

            return GetUserLicenses(filteredUsers, licenses);
        }
        
        /// <summary>
        /// Returns the license service.
        /// </summary>
        /// <returns></returns>
        private ILicenseService GetLicenseService()
        {
            using var result = _licenseServiceProvider.GetLicenseService();

            result.Settings = GetSettings(result.Settings);

            return result;
        }

        /// <summary>
        /// Returns LicenseServiceSettings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private LicenseServiceSettings GetSettings(LicenseServiceSettings settings)
        {
            settings ??= new LicenseServiceSettings();
            settings.TimeOut = _licenseSettings.Timeout;

            return settings;
        }
        
        /// <summary>
        /// Returns a collection of licence infos.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="licenseService"></param>
        /// <param name="emails"></param>
        /// <returns></returns>
        private async Task<ICollection<LicenseInfo>> GetLicensesAsync(GetReportRequest request,
            ILicenseService licenseService, ICollection<string> emails)
        {
            try
            {
                return await licenseService.GetLicensesAsync(request.DomainId, emails);
            }
            catch (Exception ex)
            {
                Log.Error($"Problem of getting licenses information: {ex.Message}");
                throw ex;
            }
        }
        
        /// <summary>
        /// Returns a dictionary with licenses by guid.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="licenses"></param>
        /// <returns></returns>
        private Dictionary<Guid, LicenseInfo> GetUserLicenses(IEnumerable<User> users, ICollection<LicenseInfo> licenses)
        {
            //Could be done with less code by using LINQ, but i think foreach code is more readable.
            var result = new Dictionary<Guid, LicenseInfo>();

            foreach (var user in users)
            {
                var licensInfo = licenses.FirstOrDefault(l => l.Email == user.UserEmail);

                if (licensInfo != default(LicenseInfo))
                {
                    result.Add(user.Id, licensInfo);
                }
            }

            return result;
        }
        #endregion
        
        /// <summary>
        /// Failed attempt handler.
        /// </summary>
        /// <returns></returns>
        private Task OnGetReportFailed()
        {
            _counter++;
            Log.Debug($"Attempt {_counter} failed");
            return null;
        }
        
        /// <summary>
        /// Attempts exceed handler.
        /// </summary>
        /// <returns></returns>
        private IActionResult OnAttemptsExceed()
        {
            return new StatusCodeResult(500);
        }
    }
}
