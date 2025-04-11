using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VillageNewbies.ViewModels
{
    // ViewModelBase toimii kaikkien ViewModeleiden yhteisenä kantaluokkana
    public class ViewModelBase : INotifyPropertyChanged
    {
        // INotifyPropertyChanged toteutus
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged-metodi, jota kutsutaan kun propertyn arvo muuttuu
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // SetProperty-metodi, joka asettaa arvon ja kutsuu OnPropertyChanged-metodia
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 