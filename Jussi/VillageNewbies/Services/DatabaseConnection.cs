using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace VillageNewbies.Services
{
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance;
        private readonly string _connectionString;
        private readonly string _connectionName = "Jussin yhteys";

        // Singleton pattern
        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseConnection();
                }
                return _instance;
            }
        }

        // Konstruktori yhteyden muodostusta varten
        private DatabaseConnection()
        {
            // Connection string muodostus MySQL:ää varten
            _connectionString = "Server=127.0.0.1;Port=3307;Database=vn;Uid=root;Pwd=Ruutti;";
        }

        // Metodi yhteyden avaamiseen
        private MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.ConnectionString = _connectionString;
            return connection;
        }

        // Yhteyden nimi
        public string ConnectionName => _connectionName;

        // Yleiskäyttöinen metodi, joka suorittaa annetun SQL-kyselyn ja palauttaa taulun
        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Parametrien lisäys kyselyyn
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    // Datan lukeminen tauluun
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));
                        return dataTable;
                    }
                }
            }
        }

        // Metodi ei-hakutoiminnoille (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Parametrien lisäys kyselyyn
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    // Suoritetaan kysely ja palautetaan vaikutettujen rivien määrä
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Metodi yksittäisen arvon hakemiseen (esim. COUNT)
        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Parametrien lisäys kyselyyn
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    // Suoritetaan kysely ja palautetaan skalaari tulos
                    return await command.ExecuteScalarAsync();
                }
            }
        }
    }
} 