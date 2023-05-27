using Android.OS;
using FileShareConnectivity.Platforms.Android.WifiDirect;

namespace FileShareConnectivity.Platforms.Android;

public class BaseShareActivity : MauiAppCompatActivity
{
    private NetworkProxy _networkProxy;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (_networkProxy == null)
        {
            _networkProxy = new NetworkProxy(this);
            FileSharingWrapper fileSharingWrapper = (FileSharingWrapper)MauiApplication.Current.Services.GetService(typeof(FileSharingWrapper));
            fileSharingWrapper.NetworkService = _networkProxy;
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        _networkProxy?.RegisterReceiver();
    }

    protected override void OnPause()
    {
        base.OnPause();
        _networkProxy?.CleanAnyConnections();
    }
}
