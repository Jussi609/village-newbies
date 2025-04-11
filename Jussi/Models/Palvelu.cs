using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class Palvelu : INotifyPropertyChanged
    {
        private int _palvelu_id;
        private int _alue_id;
        private string _nimi;
        private string _kuvaus;
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

        public int Alue_id
        {
            get => _alue_id;
            set
            {
                if (_alue_id != value)
                {
                    _alue_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Nimi
        {
            get => _nimi;
            set
            {
                if (_nimi != value)
                {
                    _nimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Kuvaus
        {
            get => _kuvaus;
            set
            {
                if (_kuvaus != value)
                {
                    _kuvaus = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged(nameof(HintaSisaltaaALV));
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
                    OnPropertyChanged(nameof(HintaSisaltaaALV));
                }
            }
        }

        // Lasketaan hinta sisältäen ALV
        public double HintaSisaltaaALV => Hinta * (1 + Alv / 100);

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 