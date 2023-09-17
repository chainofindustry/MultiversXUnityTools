using System;

namespace Mx.NET.SDK.Domain.Helper
{
    public static class TimestampConverter
    {
        /// <summary>
        /// Converts the timestamp to datetime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>UTC datetime</returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp).ToUniversalTime();
        }

        /// <summary>
        /// Converts the datetime to timestamp
        /// </summary>
        /// <param name="dateTime">UTC DateTime</param>
        /// <returns>timestamp</returns>
        public static long ToTimestamp(this DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)dateTime.Subtract(origin).TotalSeconds;
        }
    }
}
