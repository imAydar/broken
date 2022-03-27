using System;

namespace BrokenCode.Model
{
    public class GetBackupEnabledByDomainAsyncParam
    {
        /// <summary>
        /// Id of domain.
        /// </summary>
        public Guid DomainId { get; set; }

        /// <summary>
        /// Backup enabled flag.
        /// </summary>
        public bool BackupEnabled { get; set; }

        /// <summary>
        /// User state.
        /// </summary>
        public UserState State { get; set; }
    }
}
