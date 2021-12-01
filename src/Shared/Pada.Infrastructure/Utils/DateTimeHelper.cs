using System;

namespace Pada.Infrastructure.Utils
{
    public static class DateTimeHelper
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}