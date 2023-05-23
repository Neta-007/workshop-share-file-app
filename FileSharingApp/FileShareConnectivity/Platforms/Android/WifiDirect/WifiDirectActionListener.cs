using Android.Net.Wifi.P2p;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

internal class WifiDirectActionListener : Java.Lang.Object, WifiP2pManager.IActionListener
{
    private readonly Action _onSuccess;
    private readonly Action<WifiP2pFailureReason> _onFailure;

    public WifiDirectActionListener(Action onSuccess, Action<WifiP2pFailureReason> onFailure)
    {
        _onSuccess = onSuccess;
        _onFailure = onFailure;
    }

    public void OnSuccess()
    {
        _onSuccess?.Invoke();
    }

    public void OnFailure(WifiP2pFailureReason reason)
    {
        _onFailure?.Invoke(reason);
    }
}
