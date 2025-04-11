using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    // Tämä luokka edustaa raporttiin tarvittavaa palveluriviä
    public class PalveluRaportti : INotifyPropertyChanged
    {
        private int _palvelu_id;
        private string _palvelun_nimi;
        private string _alueen_nimi;
        private DateTime _varattu_alkupvm;
        private DateTime _varattu_loppupvm;
        private string _asiakkaan_nimi;
        private int _lkm;
        private double _hinta;
        private double _alv;

        public int Palvelu_id
        {
            get => _palvelu_id;
            set
            {
                if (_palvelu_id != value)
                {
                    _palvelu_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PalvelunNimi
        {
            get => _palvelun_nimi;
            set
            {
                if (_palvelun_nimi != value)
                {
                    _palvelun_nimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AlueenNimi
        {
            get => _alueen_nimi;
            set
            {
                if (_alueen_nimi != value)
                {
                    _alueen_nimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime VarattuAlkupvm
        {
            get => _varattu_alkupvm;
            set
            {
                if (_varattu_alkupvm != value)
                {
                    _varattu_alkupvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime VarattuLoppupvm
        {
            get => _varattu_loppupvm;
            set
            {
                if (_varattu_loppupvm != value)
                {
                    _varattu_loppupvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AsiakaanNimi
        {
            get => _asiakkaan_nimi;
            set
            {
                if (_asiakkaan_nimi != value)
                {
                    _asiakkaan_nimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Lkm
        {
            get => _lkm;
            set
            {
                if (_lkm != value)
                {
                    _lkm = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Yhteishinta));
                }
            }
        }

        public double Hinta
        {
            get => _hinta;
            set
            {
                if (_hinta != value)
                {
                    _hinta = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Yhteishinta));
                }
            }
        }

        public double Alv
        {
            get => _alv;
            set
            {
                if (_alv != value)
                {
                    _alv = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Yhteishinta));
                }
            }
        }

        // Palvelun yhteishinta (määrä * hinta sis. ALV)
        public double Yhteishinta => Lkm * Hinta * (1 + Alv / 100);

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 