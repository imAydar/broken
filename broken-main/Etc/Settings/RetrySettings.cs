namespace BrokenCode.Etc.Settings
{
    /// <summary>
    /// Common settings.
    /// </summary>
    public class RetrySettings
    {
        /// <summary>
        /// Number of attempts.
        /// </summary>
        public int AttemptsNumber { get; } = 10;

        /// <summary>
        /// Retry interval in ms.
        /// </summary>
        public int Interval { get; } = 1000;
    }
}

