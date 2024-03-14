using System.Diagnostics;

namespace KYC360_InternshipAssessment.Helpers
{
    public static class Retry
    {
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action,
            Func<T, bool> validateResult = null,
            int maxRetries = 5, int maxDelayMilliseconds = 5000, int delayMilliseconds = 500)

        {
            var backoff = new ExponentialBackoff(delayMilliseconds, maxDelayMilliseconds);

            var exceptions = new List<Exception>();

            for (var retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var result = await action()
                        .ConfigureAwait(false);
                    var isValid = validateResult?.Invoke(result);
                    if (isValid.HasValue && isValid.Value)
                    {
                        Debug.WriteLine(result);
                        return result;
                    }

                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    await backoff.Delay()
                        .ConfigureAwait(false);
                    Debug.WriteLine($"{ex.Message} | Attempt : {retry + 1} | Delay between next attempt : {backoff._currentDelay}ms");
                }
            }

            throw new AggregateException(exceptions);
        }

        private struct ExponentialBackoff
        {
            private readonly int _delayMilliseconds;
            private readonly int _maxDelayMilliseconds;
            private int _retries;
            private int _pow;
            public int _currentDelay = 0;

            public ExponentialBackoff(int delayMilliseconds, int maxDelayMilliseconds)
            {
                _delayMilliseconds = delayMilliseconds;
                _maxDelayMilliseconds = maxDelayMilliseconds;
                _retries = 0;
                _pow = 1;
            }

            /// <summary>
            /// 
            ///     - `_delayMilliseconds`: Represents the initial delay in milliseconds before the first retry.
            ///     - `_pow`: Tracks the exponent value used for exponential backoff calculation.
            ///     - `_maxDelayMilliseconds`: Represents the maximum delay allowed in milliseconds.
            ///     - The calculated delay is the minimum value between `_delayMilliseconds* (_pow - 1) / 2` (which increases exponentially with retries) and `_maxDelayMilliseconds` 
            ///        (to ensure it does not exceed the maximum allowed delay).
            /// 
            /// </summary>
            /// <returns></returns>

            public Task Delay()
            {
                ++_retries;
                if (_retries < 31)
                {
                    _pow = _pow << 1; // Increases exponentially by powers of 2.
                }

                _currentDelay = Math.Min(_delayMilliseconds * (_pow - 1) / 2, _maxDelayMilliseconds);

                return Task.Delay(_currentDelay);
            }


        }
    }
}
