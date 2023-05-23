using FileSharingApp.Interfaces;

namespace FileSharingApp;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertService;

    /* 
     * TODO:
     * If we want to register more services globaly to app, can we do it? indtead of sending it in the ctor?
     * Do we need to register more services globaly to the app?
     */

    public App(IServiceProvider provider)
	{
		InitializeComponent();
        Services = provider;
        AlertService = Services.GetService<IAlertService>();
        MainPage = new AppShell();
	}
}
