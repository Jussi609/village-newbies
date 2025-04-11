using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class Posti : INotifyPropertyChanged
    {
        private string _postinro;
        private string _toimipaikka;

        public string Postinro
        {
            get => _postinro;
            set
            {
                if (_postinro != value)
                {
                    _postinro = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Toimipaikka
        {
            get => _toimipaikka;
            set
            {
                if (_toimipaikka != value)
                {
                    _toimipaikka = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 