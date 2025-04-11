using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class Alue : INotifyPropertyChanged
    {
        private int _alue_id;
        private string _nimi;

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

        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 