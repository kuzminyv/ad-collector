using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace Core.DAL.MsSql
{
    public static class SqlDataReaderExtension
    {
        public static bool GetBoolean(this SqlDataReader reader, string name)
        {
            return Convert.ToBoolean(reader[name]);
        }

        public static int GetInt32(this SqlDataReader reader, string name)
        {
            return Convert.ToInt32(reader[name]);
        }


        public static int? GetNullableInt32(this SqlDataReader reader, string name)
        {
            if (reader[name] != DBNull.Value)
                return Convert.ToInt32(reader[name]);
            else
                return null;
        }

        public static float? GetNullableFloat(this SqlDataReader reader, string name)
        {
            if (reader[name] != DBNull.Value)
                return Convert.ToSingle(reader[name]);
            else
                return null;
        }

        public static string GetNullableString(this SqlDataReader reader, string name)
        {
            if (reader[name] != DBNull.Value)
                return Convert.ToString(reader[name]);
            else
                return null;
        }

        public static string GetNullableString(this SqlDataReader reader, int index)
        {
            if (reader[index] != DBNull.Value)
                return reader.GetString(index);
            else
                return null;
        }

        public static Int64 GetInt64(this SqlDataReader reader, string name)
        {
            return Convert.ToInt64(reader[name]);
        }

        public static Int64? GetNullableInt64(this SqlDataReader reader, string name)
        {
            if (reader[name] != DBNull.Value)
                return Convert.ToInt64(reader[name]);
            else
                return null;
        }

        public static DateTime GetDateTime(this SqlDataReader reader, string name)
        {
            return Convert.ToDateTime(reader[name]);
        }


        public static DateTime? GetNullableDateTime(this SqlDataReader reader, string name)
        {
            if (reader[name] != DBNull.Value)
                return Convert.ToDateTime(reader[name]);
            else
                return null;
        }

        public static DateTime? GetNullableDateTime(this SqlDataReader reader, int index)
        {
            if (reader[index] != DBNull.Value)
                return reader.GetDateTime(index);
            else
                return null;
        }

        public static string GetString(this SqlDataReader reader, string name)
        {
            return Convert.ToString(reader[name]);
        }

        public static double GetDouble(this SqlDataReader reader, string name)
        {
            return Convert.ToDouble(reader[name]);
        }

        public static decimal GetDecimal(this SqlDataReader reader, string name)
        {
            return Convert.ToDecimal(reader[name]);
        }

        public static object GetValue(this SqlDataReader reader, string name)
        {
            return reader[name];
        }
    }
}