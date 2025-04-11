using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.Models
{
    public class VarauksenPalvelu : INotifyPropertyChanged
    {
        private int _varaus_id;
        private int _palvelu_id;
        private int _lkm;

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

        public int Lkm
        {
            get => _lkm;
            set
            {
                if (_lkm != value)
                {
                    _lkm = value;
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