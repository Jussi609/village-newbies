using System;
using Microsoft.Maui.Controls;

namespace VillageNewbies
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Rekisteröidään reitit näkymille tarvittaessa
            Routing.RegisterRoute("asiakashallinta", typeof(Views.AsiakasPage));
            Routing.RegisterRoute("palveluraportit", typeof(Views.PalveluRaporttiPage));
        }
    }
} 