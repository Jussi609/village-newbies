using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace AlueetJaMokit
{
    public class Alue : INotifyPropertyChanged
    {

        public int AlueId { get; set; }

        //Nimitoiminnot
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

       
        //Mökit alueisiin
        public ObservableCollection<Mokki> Mokit { get; set; } = new();



        //Vetovalikko toiminnot
        private bool vetovalikkoAlue = false;
        public bool VetovalikkoAlue
        {
            get => vetovalikkoAlue;
            set
            {
                if (vetovalikkoAlue != value)
                {
                    vetovalikkoAlue = value;
                    OnPropertyChanged(nameof(VetovalikkoAlue));
                    OnPropertyChanged(nameof(ButtonTekstiVetovalikkoAlue));
                }
            }
        }
        //Muuttuva buttonin teksti
        public string ButtonTekstiVetovalikkoAlue
          => VetovalikkoAlue ? "Piilota mökit" : "Näytä mökit";

        //Käyttöliittymän muuttumistiedot
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    
    
    }
}