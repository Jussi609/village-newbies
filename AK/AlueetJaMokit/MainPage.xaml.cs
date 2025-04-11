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


        //ALUEET

        //Alueiden lisääminen napin avulla
        private async void LisaaAlue_Clicked(object sender, EventArgs e)
        {
            var uusiAlue = new Alue
            {
                AlueId = seuraavaAlueID,
                Nimi = $"Alue {seuraavaAlueID}"
            };
            seuraavaAlueID++;

            alueet.Add(uusiAlue);

            try
            {
                DatabaseConnector dbc = new DatabaseConnector();
                dbc.TallennaAlueTietokantaan(uusiAlue);
                await DisplayAlert("Onnistui", $"Alue '{uusiAlue.Nimi}' tallennettu tietokantaan.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("VIRHE!", $"Tietokantaan tallennus epäonnistui: {ex.Message}", "OK");
            }
        }




        //Alueiden poistaminen napin avulla
        private async void PoistaAlue_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Alue valittuAlue)
            {
                bool vahvistus = await DisplayAlert("Vahvista", $"Haluatko varmasti poistaa alueen '{valittuAlue.Nimi}'?", "Kyllä", "Peruuta");

                if (vahvistus)
                {
                    alueet.Remove(valittuAlue);

                    try
                    {
                        DatabaseConnector dbc = new DatabaseConnector();
                        dbc.PoistaAlueTietokannasta(valittuAlue.AlueId);

                        await DisplayAlert("Onnistui", $"Alue '{valittuAlue.Nimi}' poistettu tietokannasta.", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Virhe", $"Alueen poistaminen epäonnistui: {ex.Message}", "OK");
                    }
                }
            }
        }




        //Alueiden nimen muokkaaminen ja uuden nimen päivittäminen tietokantaan
        private async void MuokkaaAlueNimea_Clicked(object sender, EventArgs e)
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
                        

                        try
                        {
                            DatabaseConnector dbc = new DatabaseConnector();
                            dbc.PaivitaAlueTietokantaan(valittuAlue);

                            await DisplayAlert("Onnistui", $"Alueen nimi päivitetty tietokantaan.", "OK");
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Virhe", $"Alueen nimen päivitys epäonnistui: {ex.Message}", "OK");
                        }
                        break;
                    }
                }
            }
        }




        //MÖKIT


        private int seuraavaMokkiId = 1;


        //Mökkien lisääminen tietokantaan napin avulla
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

