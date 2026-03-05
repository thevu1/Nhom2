using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace xuatKhoMoi
{
    public class DBConnection
    {
        public static MySqlConnection GetConnection()
        {
            string connStr =
                "server=localhost;database=phieuxuat;uid=root;pwd=123456;SslMode=None;Charset=utf8mb4;";
            return new MySqlConnection(connStr);
        }
    }
}