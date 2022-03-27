using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BrokenCode.Interfaces;
using BrokenCode.Model;

using Microsoft.EntityFrameworkCore;

namespace BrokenCode.Repositories
{
    //Since we are assuming that only BrokenService.cs is needs to be refactored i didn't touch
    //models in service layer though they are entities at the same time. In real project i'd have
    //entities in Data layer, dto models in service layer(or another layer, it depends on
    //architecture) and they'd be connected by mappings.
    
    /// <summary>
    /// User repository.
    /// </summary>
    public class UserBaseRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// <see cref="UserDbContext"/>
        /// </summary>
        private readonly UserDbContext _context;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="context"></param>
        public UserBaseRepository(UserDbContext context) : base(context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<User>> GetBackupEnabledUsersByDomainAsync(GetBackupEnabledByDomainAsyncParam param)
        {
            var query = _context.Users
                .Include(x => x.Email)
                .Include(x => x.Drive)
                .Include(x => x.Calendar)
                .Where(x => x.DomainId == param.DomainId && x.BackupEnabled == param.BackupEnabled)
                .OrderBy(x => x.Email);

            return await query.ToListAsync();
        }
    }
}
