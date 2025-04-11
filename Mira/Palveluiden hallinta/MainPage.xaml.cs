using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Palveluiden_hallinta
{
    public class Palvelu : INotifyPropertyChanged
    {
        public int PalveluId { get; set; }
        public int AlueId { get; set; }
        public string Nimi { get; set; }
        public string Kuvaus { get; set; }
        public double Hinta { get; set; }
        public double Alv { get; set; }
        public string AlueenNimi { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Palvelu> palvelut = new ObservableCollection<Palvelu>();

        public ObservableCollection<Palvelu> Palvelut
        {
            get { return palvelut; }
            set
            {
                palvelut = value;
                OnPropertyChanged(nameof(Palvelut));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this; // Set BindingContext to the page itself
            LoadPalvelut();
        }
        private readonly string server = "127.0.0.1";
        private readonly string port = "3307"; // vaihda 3307 jos käytätte sitä!
        private readonly string uid = "root";
        private readonly string pwd = "Ruutti"; // vaihda tähän oma salasanasi!
        private readonly string database = "vn"; // käytä omaa tietokannan nimeä
        private async Task LoadPalvelut()
        {
            string connectionString = $"Server={server};Port={port};uid={uid};password={pwd};database={database}";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT p.palvelu_id, a.nimi AS AlueenNimi, p.nimi, p.kuvaus, p.hinta, p.alv
                        FROM palvelu p
                        JOIN alue a ON p.alue_id = a.alue_id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Palvelu p = new Palvelu
                            {
                                PalveluId = reader.GetInt32(0),
                                AlueenNimi = reader.GetString(1), // Fetch AlueenNimi
                                Nimi = reader.GetString(2),
                                Kuvaus = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Hinta = reader.GetDouble(4),
                                Alv = reader.GetDouble(5)
                            };
                            Palvelut.Add(p);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                await DisplayAlert("Error", $"Failed to load palvelut: {ex.Message}", "OK");
            }
        }
    }
}


