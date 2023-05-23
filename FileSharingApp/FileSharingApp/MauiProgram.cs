using FileSharingApp.ViewModel;
using Microsoft.Extensions.Logging;
using FileShareConnectivity;
using FileSharingApp.View;
using FileSharingApp.Interfaces;
using FileSharingApp.Platforms;
using FileSharingApp.Services;

namespace FileSharingApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseFileShareConnectivity()
            .UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        
        registerViewsAndViewModels(builder.Services);
        registerEssentials(builder.Services);

        return builder.Build();
	}

    private static void registerViewsAndViewModels(in IServiceCollection services)
    {
        services.AddSingleton<MainPage>();
        services.AddSingleton<ShareFilePage>();
        services.AddSingleton<ShareFileViewModel>();
    }

    private static void registerEssentials(in IServiceCollection services)
    {
        services.AddSingleton<IPermissionsService, PermissionsService>();
        services.AddSingleton<IAlertService, AlertService>();
    }
}
