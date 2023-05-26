using Android.OS;
using FileShareConnectivity.Platforms.Android.WifiDirect;

namespace FileShareConnectivity.Platforms.Android;

/* TODO:
 * Problem: More then one class can inhert from me.
 * 
 * I need to think how to handle the creating on the NetworkService class.
 * Make more sence to register NetworkService as AddTransient but we need to adapt the design according to.
 * Meaning we need to think how the ViewModel (or any other class that what to access the public API) 
 *  from the EXE project will interact with the transient instence of the NetworkService becouse he will need to get context (cross platform context...)
 *  and link it some how to the activity context for example.
 */
public class BaseShareActivity : MauiAppCompatActivity
{
    private NetworkProxy _networkProxy;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if(_networkProxy == null)   // TODO: Maybe not nessecery?
        {
            _networkProxy = new NetworkProxy(this);
            FileSharingWrapper fileSharingWrapper = (FileSharingWrapper)MauiApplication.Current.Services.GetService(typeof(FileSharingWrapper));
            fileSharingWrapper.NetworkService = _networkProxy;
            //fileSharingWrapper.FileTransferService = _networkProxy;
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
        _networkProxy?.UnregisterReceiver();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _networkProxy?.Dispose();
    }
}
