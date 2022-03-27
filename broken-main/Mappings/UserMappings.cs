using BrokenCode.Data;
using BrokenCode.Etc;
using BrokenCode.Helpers;
using BrokenCode.Model;
//using BrokenCode.Model;
using System;
using System.Collections.Generic;

namespace BrokenCode.Mappings
{
    static class UserMappings
    {
        /// <summary>
        /// Map User to UserStatistics
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guidsToLicences"></param>
        /// <returns></returns>
        public static UserStatistics ToStatistics(this User user, Dictionary<Guid, LicenseInfo> guidsToLicences)
        {
            return new UserStatistics
            {
                Id = user.Id,
                UserName = user.UserEmail,
                InBackup = user.BackupEnabled,
                EmailLastBackupStatus = user.Email.LastBackupStatus,
                EmailLastBackupDate = user.Email.LastBackupDate,
                DriveLastBackupStatus = user.Drive.LastBackupStatus,
                DriveLastBackupDate = user.Drive.LastBackupDate,
                CalendarLastBackupStatus = user.Calendar.LastBackupStatus,
                CalendarLastBackupDate = user.Calendar.LastBackupDate,
                LicenseType = LicenseHelper.GetLicenseType(user, guidsToLicences).ToString()
            };
        }
        
        /// <summary>
        /// Map GetReportRequest to GetUsersParam
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static GetBackupEnabledByDomainAsyncParam ToGetBackupEnabledUsersByDomainAsyncParam(this GetReportRequest request)
        {
            return new GetBackupEnabledByDomainAsyncParam
            {
                DomainId = request.DomainId,
                BackupEnabled = true,
                State = UserState.InDomain
            };
        }
    }
}
