using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Data;
using BrokenCode.Model;

namespace BrokenCode.Interfaces
{
    public interface IUserRepository
    {
        //Todo: name is too long, might use a simple name like GetUsers.
        /// <summary>
        /// Getting users who has their backup enabled by DomainId ordered by email.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetBackupEnabledUsersByDomainAsync(GetBackupEnabledByDomainAsyncParam param);
    }
}