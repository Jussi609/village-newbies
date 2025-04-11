using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class Asiakas : INotifyPropertyChanged
    {
        private int _asiakas_id;
        private string _postinumero;
        private string _etunimi;
        private string _sukunimi;
        private string _lahiosoite;
        private string _email;
        private string _puhelinnumero;

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

        public string Postinumero
        {
            get => _postinumero;
            set
            {
                if (_postinumero != value)
                {
                    _postinumero = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Etunimi
        {
            get => _etunimi;
            set
            {
                if (_etunimi != value)
                {
                    _etunimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Sukunimi
        {
            get => _sukunimi;
            set
            {
                if (_sukunimi != value)
                {
                    _sukunimi = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Lahiosoite
        {
            get => _lahiosoite;
            set
            {
                if (_lahiosoite != value)
                {
                    _lahiosoite = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Puhelinnumero
        {
            get => _puhelinnumero;
            set
            {
                if (_puhelinnumero != value)
                {
                    _puhelinnumero = value;
                    OnPropertyChanged();
                }
            }
        }

        // Property joka näyttää asiakkaan koko nimen
        public string KokoNimi => $"{Etunimi} {Sukunimi}";

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 