using FileSharingApp.View;

namespace FileSharingApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(ShareFilePage), typeof(ShareFilePage));
    }
}
