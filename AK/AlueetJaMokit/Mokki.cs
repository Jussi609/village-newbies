using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AlueetJaMokit
{
    public class Mokki : INotifyPropertyChanged
    {
        public int MokkiId { get; set; }
        public int AlueId { get; set; }

        private string mokkinimi;
        public string Mokkinimi
        {
            get => mokkinimi;
            set
            {
                if (mokkinimi != value)
                {
                    mokkinimi = value;
                    OnPropertyChanged(nameof(Mokkinimi));
                }
            }
        }

        private string postinumero = "";
        public string Postinumero
        {
            get => postinumero;
            set
            {
                if (postinumero != value)
                {
                    postinumero = value;
                    OnPropertyChanged(nameof(Postinumero));
                }
            }
        }

        private string katuosoite = "";
        public string Katuosoite
        {
            get => katuosoite;
            set
            {
                if (katuosoite != value)
                {
                    katuosoite = value;
                    OnPropertyChanged(nameof(Katuosoite));
                }
            }
        }

        private double hinta;
        public double Hinta
        {
            get => hinta;
            set
            {
                if (hinta != value)
                {
                    hinta = value;
                    OnPropertyChanged(nameof(Hinta));
                }
            }
        }

        private string kuvaus = "Tähän mökin tiedot";
        public string Kuvaus
        {
            get => kuvaus;
            set
            {
                if (kuvaus != value)
                {
                    kuvaus = value;
                    OnPropertyChanged(nameof(Kuvaus));
                }
            }
        }

        private int henkilomaara;
        public int Henkilomaara
        {
            get => henkilomaara;
            set
            {
                if (henkilomaara != value)
                {
                    henkilomaara = value;
                    OnPropertyChanged(nameof(Henkilomaara));
                }
            }
        }

        private string varustelu = "";
        public string Varustelu
        {
            get => varustelu;
            set
            {
                if (varustelu != value)
                {
                    varustelu = value;
                    OnPropertyChanged(nameof(Varustelu));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
