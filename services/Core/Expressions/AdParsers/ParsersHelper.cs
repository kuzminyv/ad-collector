using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGX = System.Text.RegularExpressions;

namespace Core.Expressions.AdParsers
{
    public class ParsersHelper
    {
        public static string RemoveAny(string str, params string[] strToRemove)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            foreach (var s in strToRemove)
            {
                str = str.Replace(s, "");
            }
            return str;
        }

        public static string NumbersOnly(string str, params char[] allowedChars)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str[i]) || allowedChars.Any(c => c == str[i]))
                {
                    stringBuilder.Append(str[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public static string RegexGroupValue(string str, string regex, string groupName)
        {
            RGX.Regex rgx = new RGX.Regex(regex);
            var matches = rgx.Matches(str);
            if (matches.Count == 1)
            {
                return matches[0].Groups[groupName].Value;
            }
            return string.Empty;
        }

        public static int ParseInt(string str, bool onlyDigits = true, params string[] strToRemove)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(int);
            }
            if (onlyDigits)
            {
                str = NumbersOnly(str);
            }
            if (strToRemove.Length > 0)
            {
                str = RemoveAny(str, strToRemove);
            }
            if (string.IsNullOrEmpty(str))
            {
                return default(int);
            }
            return int.Parse(str);
        }

        public static double ParseDouble(string str, string decimalPoint = null, bool onlyDigits = true, params string[] strToRemove)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(double);
            }
            if (!string.IsNullOrEmpty(decimalPoint))
            {
                str = str.Replace(decimalPoint, ".");
            }
            if (onlyDigits)
            {
                char[] allowedChars = string.IsNullOrEmpty(decimalPoint) ? new char[0] : new char[] {'.'}; 
                str = NumbersOnly(str, allowedChars);
            }
            if (strToRemove.Length > 0)
            {
                str = RemoveAny(str, strToRemove);
            }
            if (string.IsNullOrEmpty(str))
            {
                return default(int);
            }
            return double.Parse(str, CultureInfo.InvariantCulture);
        }

        public static float ParseFloat(string str, string decimalPoint = null, bool onlyDigits = true, params string[] strToRemove)
        {
            return (float)ParseDouble(str, decimalPoint, onlyDigits, strToRemove);
        }

        public static bool TryParseDate(out DateTime date, string str, string dateFormat, string timeFormat, CultureInfo cultureInfo, params string[] dayNames)
        {
            var culture = cultureInfo ?? CultureInfo.InvariantCulture;
            for (int i = 0; i < dayNames.Length; i++)
            {
                if (str.IndexOf(dayNames[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    date = DateTime.Now.Date.AddDays(-i); 

                    str = NumbersOnly(str, ':', ' ').Trim();
                    if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(timeFormat))
                    {
                        DateTime time = DateTime.ParseExact(str, timeFormat, culture);
                        date = date.AddHours(time.Hour);
                        date = date.AddMinutes(time.Minute);
                    }
                    return true;
                }
            }
            return DateTime.TryParseExact(str.Trim(), dateFormat + " " + timeFormat, culture, DateTimeStyles.None, out date);
        }

        public static DateTime ParseDate(string str, string dateFormat, string timeFormat, CultureInfo cultureInfo, params string[] dayNames)
        {
            DateTime date;
            if (TryParseDate(out date, str, dateFormat, timeFormat, cultureInfo, dayNames))
            {
                return date;
            }
            throw new ArgumentException("str", string.Format("\'{0}\' is not date in valid format.", str));
        }

        public static DateTime ParseDate(string str, string[] dateFormat, string timeFormat, CultureInfo cultureInfo, params string[] dayNames)
        {
            for (int i = 0; i < dateFormat.Length; i++)
            {
                DateTime date;
                if (TryParseDate(out date, str, dateFormat[i], timeFormat, cultureInfo, dayNames))
                {
                    return date;
                }
            }
            throw new ArgumentException("str", string.Format("\'{0}\' is not date in valid format.", str));
        }


        public static DateTime ParseDate(string str, string format)
        {
            return DateTime.ParseExact(str.Trim(), format, CultureInfo.InvariantCulture);
        }
    }
}
