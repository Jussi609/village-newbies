using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VillageNewbies.Helpers;
using VillageNewbies.Models;
using VillageNewbies.Services;

namespace VillageNewbies.ViewModels
{
    public class PalveluRaporttiViewModel : ViewModelBase
    {
        private readonly PalveluRaporttiService _palveluRaporttiService;
        private ObservableCollection<PalveluRaportti> _ostetutPalvelut;
        private ObservableCollection<Palvelu> _eiVaratutPalvelut;
        private ObservableCollection<Alue> _alueet;
        private Alue _valittuAlue;
        private DateTime _alkuPvm;
        private DateTime _loppuPvm;
        private double _yhteishinta;
        private string _virheViesti;
        private bool _lataa;

        public PalveluRaporttiViewModel()
        {
            _palveluRaporttiService = new PalveluRaporttiService();
            OstetutPalvelut = new ObservableCollection<PalveluRaportti>();
            EiVaratutPalvelut = new ObservableCollection<Palvelu>();
            Alueet = new ObservableCollection<Alue>();
            
            // Alustetaan päivämäärät (tämä kuukausi)
            AlkuPvm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            LoppuPvm = AlkuPvm.AddMonths(1).AddDays(-1);
            
            // Komennot
            HaeRaporttiCommand = new RelayCommand(async param => await HaeRaporttiAsync());
            HaeAlueetCommand = new RelayCommand(async param => await HaeAlueetAsync());

            // Haetaan alueet automaattisesti näkymän alustuksessa
            Task.Run(async () => await HaeAlueetAsync());
        }

        // Properties
        public ObservableCollection<PalveluRaportti> OstetutPalvelut
        {
            get => _ostetutPalvelut;
            set => SetProperty(ref _ostetutPalvelut, value);
        }

        public ObservableCollection<Palvelu> EiVaratutPalvelut
        {
            get => _eiVaratutPalvelut;
            set => SetProperty(ref _eiVaratutPalvelut, value);
        }

        public ObservableCollection<Alue> Alueet
        {
            get => _alueet;
            set => SetProperty(ref _alueet, value);
        }

        public Alue ValittuAlue
        {
            get => _valittuAlue;
            set => SetProperty(ref _valittuAlue, value);
        }

        public DateTime AlkuPvm
        {
            get => _alkuPvm;
            set => SetProperty(ref _alkuPvm, value);
        }

        public DateTime LoppuPvm
        {
            get => _loppuPvm;
            set => SetProperty(ref _loppuPvm, value);
        }

        public double Yhteishinta
        {
            get => _yhteishinta;
            set => SetProperty(ref _yhteishinta, value);
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
        public ICommand HaeRaporttiCommand { get; private set; }
        public ICommand HaeAlueetCommand { get; private set; }

        // Metodit
        private async Task HaeAlueetAsync()
        {
            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                
                var alueet = await _palveluRaporttiService.GetAllAlueetAsync();
                
                // Lisätään "Kaikki alueet" -vaihtoehto
                alueet.Insert(0, new Alue { Alue_id = 0, Nimi = "Kaikki alueet" });
                
                Alueet = new ObservableCollection<Alue>(alueet);
                ValittuAlue = Alueet[0]; // Valitaan oletuksena "Kaikki alueet"
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe alueiden haussa: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }

        private async Task HaeRaporttiAsync()
        {
            if (AlkuPvm > LoppuPvm)
            {
                VirheViesti = "Alkupäivämäärä ei voi olla loppupäivämäärän jälkeen.";
                return;
            }

            try
            {
                Lataa = true;
                VirheViesti = string.Empty;
                
                int alueId = ValittuAlue?.Alue_id ?? 0;
                
                // Haetaan ostetut palvelut
                var ostetutPalvelut = await _palveluRaporttiService.GetOstetutPalvelutRaporttiAsync(AlkuPvm, LoppuPvm, alueId);
                OstetutPalvelut = new ObservableCollection<PalveluRaportti>(ostetutPalvelut);
                
                // Haetaan ei-varatut palvelut
                var eiVaratutPalvelut = await _palveluRaporttiService.GetEiVaratutPalvelutAsync(AlkuPvm, LoppuPvm, alueId);
                EiVaratutPalvelut = new ObservableCollection<Palvelu>(eiVaratutPalvelut);
                
                // Lasketaan yhteishinta
                Yhteishinta = _palveluRaporttiService.LaskeRaportinYhteishinta(ostetutPalvelut);
            }
            catch (Exception ex)
            {
                VirheViesti = $"Virhe raportin haussa: {ex.Message}";
            }
            finally
            {
                Lataa = false;
            }
        }
    }
} 