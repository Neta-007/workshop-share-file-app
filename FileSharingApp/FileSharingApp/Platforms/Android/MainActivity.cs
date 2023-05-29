using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Lifecycle;
using FileShareConnectivity.Platforms.Android;
using FileSharingApp.View;
using FileSharingApp.ViewModel;

namespace FileSharingApp;

[Activity(
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true,
    ScreenOrientation = ScreenOrientation.Portrait,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | 
    ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

[IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "*/*")]
public class MainActivity : BaseShareActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

    }
}
