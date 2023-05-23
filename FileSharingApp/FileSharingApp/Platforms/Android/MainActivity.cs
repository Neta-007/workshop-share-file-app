using Android.App;
using Android.Content.PM;
using Android.OS;
using FileShareConnectivity.Platforms.Android;
using FileSharingApp.View;

namespace FileSharingApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : BaseShareActivity//: MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Microsoft.Maui.Controls.Shell.Current.GoToAsync($"{nameof(ShareFilePage)}");

        /*if (_fileSharingWrapper == null)
        {
            _fileSharingWrapper = (FileSharingWrapper)MauiApplication.Current.Services.GetService(typeof(FileSharingWrapper));
        }*/

        /*if (!isWifiDirectSupportedAndEnabled())
        {
            // Wifi Direct is not supported on this device or is not enabled on the device => so don't create _networkProxy object
        }
        else
        {
            if (_networkProxy == null)
            {
                _networkProxy = new NetworkProxy(this);
            } 
        }*/
    }
}
