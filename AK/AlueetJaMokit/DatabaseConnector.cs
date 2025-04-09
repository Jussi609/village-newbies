using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySqlConnector;

namespace AlueetJaMokit
{
    public class DatabaseConnector
    {
        private readonly string server = "127.0.0.1";
        private readonly string port = "3307"; // vaihda 3307 jos käytätte sitä!
        private readonly string uid = "root";
        private readonly string pwd = "Ruutti"; // vaihda tähän oma salasanasi!
        private readonly string database = "vn"; // käytä omaa tietokannan nimeä

        public DatabaseConnector() { }

        public MySqlConnection _getConnection()
        {
            string connectionString = $"Server={server};Port={port};uid={uid};password={pwd};database={database}";
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }
    }
}

