using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueExample.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - (date.Ticks % roundTicks), date.Kind);
        }
    }
}
