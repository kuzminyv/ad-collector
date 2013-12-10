using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class UriParamsHelper
    {
        public static int? ParseInt(string str, int? defaultValue = null)
        {
            int result;
            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static double? ParseDouble(string str, double? defaultValue = null)
        {
            double result;
            if (double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static Nullable<TEnum> ParseEnum<TEnum>(string str, Nullable<TEnum> defaultValue = null)
            where TEnum: struct
        {
            int? enumValue = ParseInt(str, null);
            if (enumValue.HasValue && Enum.IsDefined(typeof(TEnum), enumValue.Value))
            {
                return (TEnum)(object)enumValue.Value;
            }
            return defaultValue;
        }
    }
}
