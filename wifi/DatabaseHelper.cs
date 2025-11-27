using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace wifi.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
        public static DataTable ExecuteSelect(string query, Dictionary<string, object>? parameters = null)
        {
            var dt = new DataTable();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    AddParameters(cmd, parameters);
                    using (var adapter = new MySqlDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            return dt;
        }
        public static object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    AddParameters(cmd, parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
        public static int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    AddParameters(cmd, parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        private static void AddParameters(MySqlCommand cmd, Dictionary<string, object>? parameters)
        {
            if (parameters == null) return;

            foreach (var kv in parameters)
            {
                string name = kv.Key.StartsWith("@") ? kv.Key : "@" + kv.Key;
                object val = kv.Value ?? DBNull.Value;
                if (val is int i)
                {
                    cmd.Parameters.Add(name, MySqlDbType.Int32).Value = i;
                }
                else if (val is decimal dec)
                {
                    var p = new MySqlParameter(name, MySqlDbType.Decimal)
                    {
                        Precision = 10,
                        Scale = 2,
                        Value = dec
                    };
                    cmd.Parameters.Add(p);
                }
                else if (val is double db)
                {
                    var p = new MySqlParameter(name, MySqlDbType.Decimal)
                    {
                        Precision = 10,
                        Scale = 2,
                        Value = Convert.ToDecimal(db)
                    };
                    cmd.Parameters.Add(p);
                }
                else
                {
                    cmd.Parameters.Add(name, MySqlDbType.VarChar).Value = val;
                }
            }
        }
    }
}
