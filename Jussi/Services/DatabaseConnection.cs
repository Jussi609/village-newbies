using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using System.Threading.Tasks;

namespace VillageNewbies.Services
{
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance;
        private readonly string _connectionString;

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
            // Connection string muodostus MariaDB:tä varten
            _connectionString = "Server=localhost;Port=3306;Database=villagedb;Uid=root;Pwd=root;";
        }

        // Metodi yhteyden avaamiseen
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

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