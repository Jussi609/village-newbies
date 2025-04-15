using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;


namespace AlueetJaMokit
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Alue> alueet = new ObservableCollection<Alue>();
        private int seuraavaAlueID = 1;
        public ObservableCollection<Alue> Alueet => alueet;


        private ObservableCollection<Alue> haettavatAlueet = new();
        public ObservableCollection<Alue> HaettavatAlueet => haettavatAlueet;


        public ObservableCollection<Mokki> Mokit => mokit;
        private ObservableCollection<Mokki> mokit = new();


        public MainPage()
        {

            InitializeComponent();
            

            DatabaseConnector dbc = new DatabaseConnector();

            List<Alue> ladatutAlueet = dbc.HaeAlueetTietokannasta();

            foreach (var alue in ladatutAlueet)
            {
                alueet.Add(alue);
                haettavatAlueet.Add(alue);
            }
            BindingContext = this;

        }


        //Vetovalikko alueelle
        private void AvaaSuljeAlue_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Alue alue)
            {
                alue.VetovalikkoAlue = !alue.VetovalikkoAlue;
            }
        }



        //Vetovalikko mökeille
        private void AvaaSuljeMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Mokki mokki)
            {
                mokki.VetovalikkoMokki = !mokki.VetovalikkoMokki;
            }
        }


        //HAKU
        private void HaeKaikki_Clicked(object sender, EventArgs e)
        {

            string alueHaku = AlueHakuEntry.Text?.Trim().ToLower();
            string mokkiHaku = MokkiHakuEntry?.Text?.Trim().ToLower();
            string postiHaku = PostiHakuEntry?.Text?.Trim().ToLower();
            string osoiteHaku = OsoiteHakuEntry?.Text?.Trim().ToLower();



            //Hakutoiminnon virheilmoitus jos ei ole laitettu haettavaa sanaa
            bool annettuHakusana =
                !string.IsNullOrWhiteSpace(alueHaku) ||
                !string.IsNullOrWhiteSpace(mokkiHaku) ||
                !string.IsNullOrWhiteSpace(postiHaku) ||
                !string.IsNullOrWhiteSpace(osoiteHaku);

            if ( !annettuHakusana )
            {
                DisplayAlert("VIRHE", "Anna hakusana", "OK");
                return;
            }


            //Haun tuloksien ilmoitukset
            haettavatAlueet.Clear();

            foreach (var alue in alueet)
            {
                bool alueLoytyy = false;

                if (!string.IsNullOrWhiteSpace(alueHaku) && alue.Nimi.ToLower().Contains(alueHaku))
                {
                    alueLoytyy = true;
                }

                var loytyvatMokit = alue.Mokit?.Where(m =>
                    (!string.IsNullOrWhiteSpace(mokkiHaku) && m.Mokkinimi.ToLower().Contains(mokkiHaku)) ||
                    (!string.IsNullOrWhiteSpace(postiHaku) && m.Postinumero.Contains(postiHaku)) ||
                    (!string.IsNullOrWhiteSpace(osoiteHaku) && m.Katuosoite.ToLower().Contains(osoiteHaku))
                ).ToList();


                bool loytyiMokkeja = loytyvatMokit != null && loytyvatMokit.Any();

                if (alueLoytyy || loytyiMokkeja)
                {
                    var uusiAlue = new Alue
                    {
                        AlueId = alue.AlueId,
                        Nimi = alue.Nimi,
                        Mokit = alueLoytyy && !loytyiMokkeja
                            ? alue.Mokit
                            : new ObservableCollection<Mokki>(loytyvatMokit)
                    };

                    haettavatAlueet.Add(uusiAlue);
                }
            }

        }


        //Tyhjennä haku
        private void TyhjennaHaku_Clicked(object sender, EventArgs e)
        {
            //Tyhjennetään hakukentät
            AlueHakuEntry.Text = "";
            MokkiHakuEntry.Text = "";
            PostiHakuEntry.Text = "";
            OsoiteHakuEntry.Text = "";

            //Palautetaan näkymä alkutilaan
            haettavatAlueet.Clear();
            foreach (var alue in alueet)
            {
                haettavatAlueet.Add(alue);
            }
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

            alueet.Insert(0,uusiAlue);
            haettavatAlueet.Insert(0,uusiAlue);

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
                    
                    try
                    {
                        DatabaseConnector dbc = new DatabaseConnector();
                        dbc.PoistaAlueTietokannasta(valittuAlue.AlueId);

                        Alueet.Remove(valittuAlue);

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
        private async void LisaaMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Alue alue)
            {
                var uusiMokki = new Mokki
                {
                    MokkiId = 0,
                    AlueId = alue.AlueId,
                    Mokkinimi = $"Mökki {alue.Mokit.Count + 1}",
                    Kuvaus = "Mökin tiedot",
                    Postinumero = "00000", 
                    Katuosoite= "Katuosoite",
                    Hinta = 0,
                    Henkilomaara = 0,
                    Varustelu = "Mökin varustelu"
                };

                alue.Mokit.Insert(0,uusiMokki);

                try
                {
                    DatabaseConnector dbc = new DatabaseConnector();
                    dbc.TallennaMokkiTietokantaan(uusiMokki);

                    await DisplayAlert("Onnistui", $"Mökki '{uusiMokki.Mokkinimi}' tallennettu tietokantaan.", "OK");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Virhe", $"Mökin tallennus epäonnistui: {ex.Message}", "OK");

                }
            }
        }



        //Mökkien poistaminen tietokannasta napin avulla
        private async void PoistaMokki_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Mokki mokki)
            {
                bool vahvistus = await DisplayAlert("Vahvista", $"Haluatko varmasti poistaa mökin '{mokki.Mokkinimi}'?", "Kyllä", "Peruuta");

                if (vahvistus)
                {
                    try
                    {
                        DatabaseConnector dbc = new DatabaseConnector();
                        dbc.PoistaMokkiTietokannasta(mokki.MokkiId);


                        // Etsi se alue jonka mökeistä tämä löytyy
                        Alue alue = Alueet.FirstOrDefault(a => a.Mokit.Contains(mokki));
                        if (alue != null)
                        {
                            alue.Mokit.Remove(mokki);
                        }

                        await DisplayAlert("Onnistui", $"Mökki '{mokki.Mokkinimi}' poistettiin.", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("VIRHE", $"Mökin poisto epäonnistui: {ex.Message}", "OK");
                    }
                }
            }
        }

       

        //Mökkien talnnetaminen tietokantaan napin avulla
        private async void TallennaMokki_Clicked(object sender, EventArgs e)
        {

            if (sender is Button button && button.CommandParameter is Mokki mokki)
            {
                try
                {
                    DatabaseConnector dbc = new DatabaseConnector();
                    dbc.TallennaMokkiTietokantaan(mokki);

                    await DisplayAlert("Tallennus onnistui", $"Mökki '{mokki.Mokkinimi}' tallennettiin.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Virhe", $"Tallennus epäonnistui: {ex.Message}", "OK");
                }
            }
        }






       







    }
}

