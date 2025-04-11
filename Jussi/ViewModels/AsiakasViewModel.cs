using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VillageNewbies.Helpers;
using VillageNewbies.Models;
using VillageNewbies.Services;

namespace VillageNewbies.ViewModels
{
    public class AsiakasViewModel : ViewModelBase
    {
        private readonly AsiakasService _asiakasService;
        private ObservableCollection<Asiakas> _asiakkaat;
        private Asiakas _valittuAsiakas;
        private Asiakas _uusiAsiakas;
        private string _hakusana;
        private bool _onTallennettu;
        private string _virheViesti;
        private bool _lataa;

        public AsiakasViewModel()
        {
            _asiakasService = new AsiakasService();
            UusiAsiakas = new Asiakas();
            Asiakkaat = new ObservableCollection<Asiakas>();
            
            // Komennot
            HaeAsiakkaatCommand = new RelayCommand(async param => await HaeAsiakkaatAsync());
            HaeAsiakkaatHakusanallaCommand = new RelayCommand(async param => await HaeAsiakkaatHakusanallaAsync());
            LisaaAsiakasCommand = new RelayCommand(async param => await LisaaAsiakasAsync());
            MuokkaaAsiakasCommand = new RelayCommand(async param => await MuokkaaAsiakasAsync());
            PoistaAsiakasCommand = new RelayCommand(async param => await PoistaAsiakasAsync(), param => ValittuAsiakas != null);
            TyhjennaLomakeCommand = new RelayCommand(param => TyhjennaLomake());

            // Haetaan asiakkaat automaattisesti näkymän alustuksessa
            Task.Run(async () => await HaeAsiakkaatAsync());
        }

        // Properties
        public ObservableCollection<Asiakas> Asiakkaat
        {
            get => _asiakkaat;
            set => SetProperty(ref _asiakkaat, value);
        }

        public Asiakas ValittuAsiakas
        {
            get => _valittuAsiakas;
            set
            {
                if (SetProperty(ref _valittuAsiakas, value) && value != null)
                {
                    // Kun asiakas valitaan, kopioidaan tiedot muokattavaan asiakkaaseen
                    UusiAsiakas = new Asiakas
                    {
                        Asiakas_id = value.Asiakas_id,
                        Etunimi = value.Etunimi,
                        Sukunimi = value.Sukunimi,
                        Postinumero = value.Postinumero,
                        Lahiosoite = value.Lahiosoite,
                        Email = value.Email,
                        Puhelinnumero = value.Puhelinnumero
                    };
                }
            }
        }

        public Asiakas UusiAsiakas
        {
            get => _uusiAsiakas;
            set => SetProperty(ref _uusiAsiakas, value);
        }

        public string Hakusana
        {
            get => _hakusana;
            set => SetProperty(ref _hakusana, value);
        }

        public bool OnTallennettu
        {
            get => _onTallennettu;
            set => SetProperty(ref _onTallennettu, value);
        }

        public string VirheViesti
        {
            get => _virheViesti;
            set => SetProperty(ref _virheViesti, value);
        }

        public bool Lataa
        {
            get => _lataa;
            set => SetProperty(ref _lataa, value);
        }

        // Komennot
        public ICommand HaeAsiakkaatCommand { get; private set; }
        public ICommand HaeAsiakkaatHakusanallaCommand { get; private set; }
        public ICommand LisaaAsiakasCommand { get; private set; }
        public ICommand MuokkaaAsiakasCommand { get; private set; }
        public ICommand PoistaAsiakasCommand { get; private set; }
        public ICommand TyhjennaLomakeCommand { get; private set; }

        // Metodit
        private async Task HaeAsiakkaatAsync()
        {
            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                var asiakkaat = await _asiakasService.GetAllAsiakkaatAsync();
                Asiakkaat = new ObservableCollection<Asiakas>(asiakkaat);
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe asiakkaiden haussa: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private async Task HaeAsiakkaatHakusanallaAsync()
        {
            if (string.IsNullOrWhiteSpace(Hakusana))
            {
                await HaeAsiakkaatAsync();
                return;
            }

            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                var asiakkaat = await _asiakasService.SearchAsiakkaatAsync(Hakusana);
                Asiakkaat = new ObservableCollection<Asiakas>(asiakkaat);
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe asiakkaiden haussa: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private async Task LisaaAsiakasAsync()
        {
            if (!ValidoiAsiakas())
            {
                return;
            }

            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                
                UusiAsiakas.Asiakas_id = 0; // Varmistetaan että id on 0 uudelle asiakkaalle
                int newId = await _asiakasService.AddAsiakasAsync(UusiAsiakas);
                
                if (newId > 0)
                {
                    UusiAsiakas.Asiakas_id = newId;
                    Asiakkaat.Add(UusiAsiakas);
                    OnTallennettu = true;
                    TyhjennaLomake();
                }
                else
                {
                    VirheViesti = "Asiakkaan lisääminen epäonnistui.";
                }
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe asiakkaan lisäämisessä: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private async Task MuokkaaAsiakasAsync()
        {
            if (ValittuAsiakas == null || !ValidoiAsiakas())
            {
                return;
            }

            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                
                bool success = await _asiakasService.UpdateAsiakasAsync(UusiAsiakas);
                
                if (success)
                {
                    // Päivitetään asiakkaan tiedot listassa
                    int index = Asiakkaat.IndexOf(ValittuAsiakas);
                    if (index >= 0)
                    {
                        Asiakkaat[index] = UusiAsiakas;
                    }
                    
                    OnTallennettu = true;
                    TyhjennaLomake();
                    
                    // Haetaan päivitetyt tiedot
                    await HaeAsiakkaatAsync();
                }
                else
                {
                    VirheViesti = "Asiakkaan päivittäminen epäonnistui.";
                }
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe asiakkaan päivittämisessä: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private async Task PoistaAsiakasAsync()
        {
            if (ValittuAsiakas == null)
            {
                return;
            }

            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                
                bool success = await _asiakasService.DeleteAsiakasAsync(ValittuAsiakas.Asiakas_id);
                
                if (success)
                {
                    Asiakkaat.Remove(ValittuAsiakas);
                    TyhjennaLomake();
                }
                else
                {
                    VirheViesti = "Asiakkaan poistaminen epäonnistui. Asiakkaalla voi olla varauksia.";
                }
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe asiakkaan poistamisessa: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private void TyhjennaLomake()
        {
            UusiAsiakas = new Asiakas();
            ValittuAsiakas = null;
            OnTallennettu = false;
        }

        // Validointi
        private bool ValidoiAsiakas()
        {
            if (string.IsNullOrWhiteSpace(UusiAsiakas.Etunimi))
            {
                VirheViesti = "Etunimi on pakollinen tieto.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UusiAsiakas.Sukunimi))
            {
                VirheViesti = "Sukunimi on pakollinen tieto.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UusiAsiakas.Postinumero))
            {
                VirheViesti = "Postinumero on pakollinen tieto.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UusiAsiakas.Lahiosoite))
            {
                VirheViesti = "Lähiosoite on pakollinen tieto.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(UusiAsiakas.Email) && !IsValidEmail(UusiAsiakas.Email))
            {
                VirheViesti = "Sähköpostiosoite on virheellinen.";
                return false;
            }

            VirheViesti = string.Empty;
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}