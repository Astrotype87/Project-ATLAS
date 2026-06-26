using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectATLAS.Utility
{
    public static class TaskUtil
    {
        /// <summary>
        /// Waits asynchronously until the given condition returns true.
        /// </summary>
        public static async Task WaitUntilAsync(Func<bool> condition, TimeSpan checkInterval)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (checkInterval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(checkInterval));

            while (!condition())
            {
                await Task.Delay(checkInterval); // Non-blocking wait
            }
        }
    }
}
