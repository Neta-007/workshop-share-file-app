using Android.OS;
using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity.Platforms.Android;

public class BaseShareActivity : MauiAppCompatActivity
{
    private NetworkService _networkService;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (_networkService == null)
        {
            _networkService = (NetworkService)MauiApplication.Current.Services.GetService(typeof(INetworkService));
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        _networkService?.RegisterReceiver();
    }

    protected override void OnPause()
    {
        base.OnPause();
        _networkService?.CleanAnyOpenConnections();
    }
}
