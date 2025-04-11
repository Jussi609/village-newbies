using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VillageNewbies.Models;

namespace VillageNewbies.Services
{
    public class AsiakasService
    {
        private readonly DatabaseConnection _dbConnection;

        public AsiakasService()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        // Hae kaikki asiakkaat
        public async Task<List<Asiakas>> GetAllAsiakkaatAsync()
        {
            string query = "SELECT * FROM asiakas ORDER BY sukunimi, etunimi";
            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query);
            
            return ConvertDataTableToAsiakkaatList(dataTable);
        }

        // Hae asiakas ID:n perusteella
        public async Task<Asiakas> GetAsiakasAsync(int id)
        {
            string query = "SELECT * FROM asiakas WHERE asiakas_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query, parameters);
            
            if (dataTable.Rows.Count > 0)
            {
                return ConvertDataRowToAsiakas(dataTable.Rows[0]);
            }
            
            return null;
        }

        // Hae asiakkaita nimen perusteella
        public async Task<List<Asiakas>> SearchAsiakkaatAsync(string searchTerm)
        {
            string query = "SELECT * FROM asiakas WHERE etunimi LIKE @searchTerm OR sukunimi LIKE @searchTerm OR email LIKE @searchTerm OR puhelinnumero LIKE @searchTerm ORDER BY sukunimi, etunimi";
            var parameters = new Dictionary<string, object>
            {
                { "@searchTerm", $"%{searchTerm}%" }
            };

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToAsiakkaatList(dataTable);
        }

        // Lisää uusi asiakas
        public async Task<int> AddAsiakasAsync(Asiakas asiakas)
        {
            string query = @"
                INSERT INTO asiakas (postinumero, etunimi, sukunimi, lahiosoite, email, puhelinnumero)
                VALUES (@postinumero, @etunimi, @sukunimi, @lahiosoite, @email, @puhelinnumero);
                SELECT LAST_INSERT_ID();";

            var parameters = new Dictionary<string, object>
            {
                { "@postinumero", asiakas.Postinumero },
                { "@etunimi", asiakas.Etunimi },
                { "@sukunimi", asiakas.Sukunimi },
                { "@lahiosoite", asiakas.Lahiosoite },
                { "@email", asiakas.Email },
                { "@puhelinnumero", asiakas.Puhelinnumero }
            };

            object result = await _dbConnection.ExecuteScalarAsync(query, parameters);
            
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            
            return -1; // Virhe tapahtui
        }

        // Päivitä olemassa oleva asiakas
        public async Task<bool> UpdateAsiakasAsync(Asiakas asiakas)
        {
            string query = @"
                UPDATE asiakas
                SET postinumero = @postinumero,
                    etunimi = @etunimi,
                    sukunimi = @sukunimi,
                    lahiosoite = @lahiosoite,
                    email = @email,
                    puhelinnumero = @puhelinnumero
                WHERE asiakas_id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", asiakas.Asiakas_id },
                { "@postinumero", asiakas.Postinumero },
                { "@etunimi", asiakas.Etunimi },
                { "@sukunimi", asiakas.Sukunimi },
                { "@lahiosoite", asiakas.Lahiosoite },
                { "@email", asiakas.Email },
                { "@puhelinnumero", asiakas.Puhelinnumero }
            };

            int affectedRows = await _dbConnection.ExecuteNonQueryAsync(query, parameters);
            
            return affectedRows > 0;
        }

        // Poista asiakas
        public async Task<bool> DeleteAsiakasAsync(int id)
        {
            // Ensin tarkistetaan onko asiakkaalla varauksia
            string checkQuery = "SELECT COUNT(*) FROM varaus WHERE asiakas_id = @id";
            var checkParameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            object result = await _dbConnection.ExecuteScalarAsync(checkQuery, checkParameters);
            int count = Convert.ToInt32(result);
            
            if (count > 0)
            {
                // Asiakkaalla on varauksia, joten ei voida poistaa
                return false;
            }

            // Jos asiakkaalla ei ole varauksia, poistetaan
            string query = "DELETE FROM asiakas WHERE asiakas_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            int affectedRows = await _dbConnection.ExecuteNonQueryAsync(query, parameters);
            
            return affectedRows > 0;
        }

        // Apumetodi: Muuntaa DataTable:n Asiakas-listaksi
        private List<Asiakas> ConvertDataTableToAsiakkaatList(DataTable dataTable)
        {
            List<Asiakas> asiakkaat = new List<Asiakas>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                asiakkaat.Add(ConvertDataRowToAsiakas(row));
            }
            
            return asiakkaat;
        }

        // Apumetodi: Muuntaa DataRow:n Asiakas-olioksi
        private Asiakas ConvertDataRowToAsiakas(DataRow row)
        {
            return new Asiakas
            {
                Asiakas_id = Convert.ToInt32(row["asiakas_id"]),
                Postinumero = row["postinumero"].ToString(),
                Etunimi = row["etunimi"].ToString(),
                Sukunimi = row["sukunimi"].ToString(),
                Lahiosoite = row["lahiosoite"].ToString(),
                Email = row["email"].ToString(),
                Puhelinnumero = row["puhelinnumero"].ToString()
            };
        }

        // Hae asiakkaat toimipaikan perusteella (sisältää postitiedon)
        public async Task<List<Asiakas>> GetAsiakkaatWithPostiAsync()
        {
            string query = @"
                SELECT a.*, p.toimipaikka
                FROM asiakas a
                JOIN posti p ON a.postinumero = p.postinro
                ORDER BY a.sukunimi, a.etunimi";

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query);
            List<Asiakas> asiakkaat = new List<Asiakas>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                Asiakas asiakas = new Asiakas
                {
                    Asiakas_id = Convert.ToInt32(row["asiakas_id"]),
                    Postinumero = row["postinumero"].ToString(),
                    Etunimi = row["etunimi"].ToString(),
                    Sukunimi = row["sukunimi"].ToString(),
                    Lahiosoite = row["lahiosoite"].ToString(),
                    Email = row["email"].ToString(),
                    Puhelinnumero = row["puhelinnumero"].ToString()
                };
                
                asiakkaat.Add(asiakas);
            }
            
            return asiakkaat;
        }
    }
} 