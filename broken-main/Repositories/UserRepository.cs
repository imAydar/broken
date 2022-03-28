using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BrokenCode.Interfaces;
using BrokenCode.Model;

using Microsoft.EntityFrameworkCore;

namespace BrokenCode.Repositories
{
    //Might be better to have data logic in separate layer(project).
    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// <see cref="UserDbContext"/>
        /// </summary>
        private readonly UserDbContext _context;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(UserDbContext context) : base(context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<User>> GetUsersAsync(GetUsersAsyncParam param)
        {
            var query = _context.Users
                .Include(x => x.Email)
                .Include(x => x.Drive)
                .Include(x => x.Calendar)
                .Where(x => x.DomainId == param.DomainId
                            && x.State == param.State
                            && x.BackupEnabled == param.BackupEnabled)
                .OrderBy(x => x.Email);

            return await query.ToListAsync();
        }
    }
}
