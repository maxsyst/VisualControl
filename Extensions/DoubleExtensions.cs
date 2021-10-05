using System;
using System.Globalization;

namespace VueExample.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToGoodFormat(this double number)
        {
            if (Math.Abs(number) < 1E-22 || Math.Abs(number) > 1E22)
            {
                return String.Empty;
            }
            if ((Math.Abs(number) >= 10000 || Math.Abs(number) < 1E-2) && Math.Abs(number - 0) > 1E-20)
            {
                return number.ToString("0.00E0", CultureInfo.InvariantCulture);
            }
            return number.ToString("0.000", CultureInfo.InvariantCulture);
        }
    }
}