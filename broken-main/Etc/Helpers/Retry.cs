using System;
using System.Threading.Tasks;

namespace BrokenCode.Helpers
{
    //I'd use Poly packet for that, but we have to have a failed attempt handler and also a return handler
    //for when max attempts are exceed. I guess Poly might do that, but i'm not sure.
    /// <summary>
    /// Retry helper.
    /// </summary>
    public class Retry<TResult>
    {
        /// <summary>
        /// Max attempts count.
        /// </summary>
        private readonly int _maxAttempts;

        /// <summary>
        /// Time between tries (ms).
        /// </summary>
        private TimeSpan? _interval;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="maxAttempts">Max attempts count.</param>
        private Retry(int maxAttempts)
        {
            _maxAttempts = Math.Max(maxAttempts, 1);
        }

        /// <summary>
        /// Sets time between tries.
        /// </summary>
        /// <param name="milliseconds">Time in ms.</param>
        /// <returns><see cref="Retry{TResult}"/></returns>
        public Retry<TResult> Interval(int milliseconds)
        {
            _interval = TimeSpan.FromTicks(milliseconds * TimeSpan.TicksPerMillisecond);
            return this;
        }

        /// <summary>
        /// Creates helper.
        /// </summary>
        /// <param name="maxAttempts">Max attempts count.</param>
        /// <returns><see cref="Retry{TResult}"/></returns>
        public static Retry<TResult> Attempts(int maxAttempts)
        {
            return new Retry<TResult>(maxAttempts);
        }
        
        /// <summary>
        /// Tries to run async func.
        /// </summary>
        /// <param name="func">Async func.</param>
        /// <param name="exceptionHandler">Method to run on failed attempt.</param>
        /// <param name="maxAttemptsExceedHandler">Func to run on max attempts exceed.</param>
        /// <returns></returns>
        public async Task<TResult> Invoke(Func<Task<TResult>> func, Func<Task> exceptionHandler = null, 
            Func<TResult> maxAttemptsExceedHandler = null)
        {
            int attempt = 0;

            while (++attempt <= _maxAttempts)
            {
                try
                {
                    var result = await func();
                    return result;
                }
                catch (Exception)
                {
                    if (attempt >= _maxAttempts)
                    {
                        if (maxAttemptsExceedHandler != null)
                        {
                            return maxAttemptsExceedHandler();
                        }
                        
                        throw;
                    }

                    if (exceptionHandler != null)
                    {
                        await exceptionHandler();
                    }

                    if (_interval.HasValue)
                    {
                        await Task.Delay(_interval.Value);
                    }
                }
            }

            return default(TResult);
        }
    }
 }
