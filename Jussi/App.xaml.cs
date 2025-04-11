using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace VillageNewbies
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Asetetaan sovellusikkunan otsikko
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                #if WINDOWS
                var nativeWindow = handler.PlatformView;
                nativeWindow.Title = "Village Newbies - Mökkivarausjärjestelmä";
                #endif
            });

            // Asetetaan aloitussivu
            MainPage = new AppShell();
        }
    }
} 