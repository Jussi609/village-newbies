using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AlueetJaMokit
{
    public class Alue : INotifyPropertyChanged
    {
        public int AlueId { get; set; }

        private string nimi;
        public string Nimi
        {
            get => nimi;
            set
            {
                if (nimi != value)
                {
                    nimi = value;
                    OnPropertyChanged(nameof(Nimi));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




        public ObservableCollection<Mokki> Mokit { get; set; } = new();

    }

}