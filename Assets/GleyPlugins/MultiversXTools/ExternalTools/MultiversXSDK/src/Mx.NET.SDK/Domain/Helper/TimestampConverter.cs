using System;

namespace Mx.NET.SDK.Domain.Helper
{
    public static class TimestampConverter
    {
        public static DateTime ToDateTime(this long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp).ToUniversalTime();
        }
    }
}
