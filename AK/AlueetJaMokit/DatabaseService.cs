using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;


namespace AlueetJaMokit
{
    public class DatabaseService
    {
        private string connectionString = "Server=localhost;Database=vn;User ID=root;Password=Ruutti;";

        public async Task<bool> TestaaYhteysAsync()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Virhe", $"Yhteys epäonnistui: {ex.Message}", "OK");
                return false;
            }
        }
    }
}


