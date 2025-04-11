using Microsoft.Extensions.Logging;
using VillageNewbies.Services;
using VillageNewbies.ViewModels;
using VillageNewbies.Views;

namespace VillageNewbies;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register services
		builder.Services.AddSingleton<DatabaseConnection>();
		builder.Services.AddSingleton<AsiakasService>();
		builder.Services.AddSingleton<PalveluRaporttiService>();
		builder.Services.AddSingleton<PostiService>();

		// Register view models
		builder.Services.AddTransient<AsiakasViewModel>();
		builder.Services.AddTransient<PalveluRaporttiViewModel>();

		// Register pages
		builder.Services.AddTransient<AsiakasPage>();
		builder.Services.AddTransient<PalveluRaporttiPage>();

		return builder.Build();
	}
}
