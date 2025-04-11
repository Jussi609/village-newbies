using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VillageNewbies.Models;

namespace VillageNewbies.Services
{
    public class PostiService
    {
        private readonly DatabaseConnection _dbConnection;

        public PostiService()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        // Hae kaikki postinumerot
        public async Task<List<Posti>> GetAllPostiAsync()
        {
            string query = "SELECT * FROM posti ORDER BY postinro";
            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query);
            
            List<Posti> postiLista = new List<Posti>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                postiLista.Add(new Posti
                {
                    Postinro = row["postinro"].ToString(),
                    Toimipaikka = row["toimipaikka"].ToString()
                });
            }
            
            return postiLista;
        }

        // Lisää uusi postinumero
        public async Task<bool> AddPostiAsync(Posti posti)
        {
            string query = @"
                INSERT INTO posti (postinro, toimipaikka)
                VALUES (@postinro, @toimipaikka)";

            var parameters = new Dictionary<string, object>
            {
                { "@postinro", posti.Postinro },
                { "@toimipaikka", posti.Toimipaikka }
            };

            try
            {
                int affectedRows = await _dbConnection.ExecuteNonQueryAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (Exception)
            {
                // Virhe tapahtui, esim. postinumero on jo olemassa
                return false;
            }
        }

        // Tarkista onko postinumero olemassa
        public async Task<bool> PostinumeroExistsAsync(string postinumero)
        {
            string query = "SELECT COUNT(*) FROM posti WHERE postinro = @postinro";
            var parameters = new Dictionary<string, object>
            {
                { "@postinro", postinumero }
            };

            object result = await _dbConnection.ExecuteScalarAsync(query, parameters);
            int count = Convert.ToInt32(result);
            
            return count > 0;
        }

        // Hae postinumero
        public async Task<Posti> GetPostiAsync(string postinumero)
        {
            string query = "SELECT * FROM posti WHERE postinro = @postinro";
            var parameters = new Dictionary<string, object>
            {
                { "@postinro", postinumero }
            };

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query, parameters);
            
            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return new Posti
                {
                    Postinro = row["postinro"].ToString(),
                    Toimipaikka = row["toimipaikka"].ToString()
                };
            }
            
            return null;
        }
    }
} 