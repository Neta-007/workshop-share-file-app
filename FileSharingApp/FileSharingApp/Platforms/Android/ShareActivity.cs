using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Lifecycle;
using FileShareConnectivity.Platforms.Android;
using FileSharingApp.View;
using FileSharingApp.ViewModel;
//using static Microsoft.Maui.ApplicationModel.Platform;

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
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Intent.Action == Intent.ActionSend && Intent.Type != null)
        {
            var clipData = Intent.ClipData;
            if (clipData != null)
            {
                var uri = clipData.GetItemAt(0).Uri;
                // Handle the shared URI here
                if (uri != null)
                {
                    var viewModel = MauiApplication.Current.Services.GetRequiredService<ShareFileViewModel>();
                    viewModel.FilePath = uri.ToString();
                    
                    //await Shell.Current.GoToAsync($"{nameof(ShareFilePage)}");
                    Microsoft.Maui.Controls.Shell.Current.GoToAsync($"{nameof(ShareFilePage)}");
                }
            }
        }
        else
        { 
        }

    }
}