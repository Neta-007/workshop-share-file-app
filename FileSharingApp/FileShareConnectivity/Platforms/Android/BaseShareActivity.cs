using Android.OS;
using FileShareConnectivity.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms.Android;

public class BaseShareActivity : MauiAppCompatActivity
{
    //private ILogger<BaseShareActivity> _logger = MauiApplication.Current.Services.GetService<ILogger<BaseShareActivity>>();
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
