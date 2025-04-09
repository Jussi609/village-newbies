using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace AlueetJaMokit
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Alue> alueet = new ObservableCollection<Alue>();
        private int seuraavaAlueID = 1;

        public ObservableCollection<Alue> Alueet => alueet;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;




        }







        private void LisaaAlue_Clicked(object sender, EventArgs e)
        {
            var uusiAlue = new Alue
            {
                AlueId = seuraavaAlueID,
                Nimi = $"Alue {seuraavaAlueID}"
            };
            seuraavaAlueID++;

            alueet.Add(uusiAlue);
        }


        private void PoistaAlue_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Alue valittuAlue)
            {
                alueet.Remove(valittuAlue);
            }
        }



        private void MuokkaaAlueNimea_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button &&
                button.CommandParameter is Alue valittuAlue &&
                button.Parent is HorizontalStackLayout stack)
            {
                foreach (var child in stack.Children)
                {
                    if (child is Entry entry && !string.IsNullOrWhiteSpace(entry.Text))
                    {
                        valittuAlue.Nimi = entry.Text;
                        break;
                    }
                }
            }
        }

        private int seuraavaMokkiId = 1;
        private void LisaaMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Alue alue)
            {
                var uusiMokki = new Mokki
                {
                    MokkiId = seuraavaMokkiId++,
                    AlueId = alue.AlueId,
                    Mokkinimi = $"Mökki {alue.Mokit.Count + 1}"
                };

                alue.Mokit.Add(uusiMokki);

            }
        }




        private void PoistaMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button &&
                button.CommandParameter is Mokki poistettavaMokki)
            {
                // Etsi se alue, jonka mökeistä tämä löytyy
                var alue = Alueet.FirstOrDefault(a => a.Mokit.Contains(poistettavaMokki));
                if (alue != null)
                {
                    alue.Mokit.Remove(poistettavaMokki);
                }
            }
        }



        private void TallennaMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Mokki mokki)
            {
                
                DisplayAlert("Tallennettu", $"Mökin '{mokki.Mokkinimi}' tiedot tallennettu.", "OK");
            }
        }







        private async void OnDatabaseClicked(object sender, EventArgs e)
        {
            DatabaseConnector dbc = new DatabaseConnector();

            try
            {
                var conn = dbc._getConnection();
                conn.Open();
                await DisplayAlert("Onnistui", "Tietokantayhteys aukesi!", "OK");
                conn.Close();
            }
            catch (MySqlException ex)
            {
                await DisplayAlert("Virhe", ex.Message, "OK");
            }
        }






    }
}

