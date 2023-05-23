using Android.Content;
using Android.Net.Wifi;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

internal class Utils
{
    public static bool IsWifiEnabled()
    {
        Context context = global::Android.App.Application.Context;
        WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

        return wifiManager?.IsWifiEnabled ?? false;
    }

    public static bool IsWifiDirectSupported()
    {
        Context context = global::Android.App.Application.Context;
        WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

        return wifiManager?.IsP2pSupported ?? false;
    }
}
