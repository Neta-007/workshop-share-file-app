using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using FileSharingApp.View;
using Android.Widget;
using FileShareConnectivity;
using FileShareConnectivity.Platforms.Android;

namespace FileSharingApp;

[Activity(//Label = "File Share Activity", 
    Exported = true,
    Theme = "@style/Maui.SplashTheme",
    ScreenOrientation = ScreenOrientation.Portrait,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                        ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "*/*")]
class ShareActivity : BaseShareActivity
{
    private FileSharingWrapper _fileSharingWrapper;

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
