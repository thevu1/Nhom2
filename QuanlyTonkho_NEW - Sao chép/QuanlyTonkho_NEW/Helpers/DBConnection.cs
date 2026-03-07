using MySql.Data.MySqlClient;

namespace QuanlyTonkho_NEW.Helpers
{
    public class DBConnection
    {
        public static MySqlConnection GetConnection()
        {
            string connStr =
            "server=localhost;" +
            "database=phieuxuatnew;" +
            "uid=appuser;" +
            "pwd=123456;" +
            "SslMode=none;" +
            "AllowPublicKeyRetrieval=true;";

            return new MySqlConnection(connStr);
        }
    }
}