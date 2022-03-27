using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenCode.Cfg
{
    /// <summary>
    /// Common settings.
    /// </summary>
    public class RetrySettings
    {
        /// <summary>
        /// Number of attempts.
        /// </summary>
        public int RetryAttemptsNumber { get; } = 10;

        /// <summary>
        /// Retry interval in ms.
        /// </summary>
        public int RetryInterval { get; } = 1000;
    }
}
