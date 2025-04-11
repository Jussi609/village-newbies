using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class Varaus : INotifyPropertyChanged
    {
        private int _varaus_id;
        private int _asiakas_id;
        private int _mokki_id;
        private DateTime _varattu_pvm;
        private DateTime? _vahvistus_pvm;
        private DateTime _varattu_alkupvm;
        private DateTime _varattu_loppupvm;

        public int Varaus_id
        {
            get => _varaus_id;
            set
            {
                if (_varaus_id != value)
                {
                    _varaus_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Asiakas_id
        {
            get => _asiakas_id;
            set
            {
                if (_asiakas_id != value)
                {
                    _asiakas_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Mokki_id
        {
            get => _mokki_id;
            set
            {
                if (_mokki_id != value)
                {
                    _mokki_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Varattu_pvm
        {
            get => _varattu_pvm;
            set
            {
                if (_varattu_pvm != value)
                {
                    _varattu_pvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? Vahvistus_pvm
        {
            get => _vahvistus_pvm;
            set
            {
                if (_vahvistus_pvm != value)
                {
                    _vahvistus_pvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Varattu_alkupvm
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

        public DateTime Varattu_loppupvm
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

        // Varauksen kesto päivinä
        public int VarauksenKestoPaivina => (Varattu_loppupvm - Varattu_alkupvm).Days + 1;

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 