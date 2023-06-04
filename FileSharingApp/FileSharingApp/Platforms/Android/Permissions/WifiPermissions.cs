
using static Android.Content.ClipData;

namespace FileSharingApp.Platforms.Android.Permissions;

internal class WifiPermissions : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    private (string androidPermission, bool isRuntime)[] requiredPermissions;

    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => requiredPermissions;

    public WifiPermissions()
    {
        constractTheListOfPermissionsNeededBySDK();
    }

    private void constractTheListOfPermissionsNeededBySDK()
    {
        var result = new List<(string androidPermission, bool isRuntime)>();
        var sdk = (int)global::Android.OS.Build.VERSION.SdkInt;

        result.Add((global::Android.Manifest.Permission.AccessWifiState, true));
        result.Add((global::Android.Manifest.Permission.ChangeWifiState, true));
        result.Add((global::Android.Manifest.Permission.AccessNetworkState, true));
        result.Add((global::Android.Manifest.Permission.ChangeNetworkState, true));
        result.Add((global::Android.Manifest.Permission.Internet, true));
        result.Add((global::Android.Manifest.Permission.WriteExternalStorage, true));
        
        if (sdk >= 33)
        {
            result.Add((global::Android.Manifest.Permission.NearbyWifiDevices, true));
            result.Add((global::Android.Manifest.Permission.ReadMediaAudio, true));
            result.Add((global::Android.Manifest.Permission.ReadMediaImages, true));
            result.Add((global::Android.Manifest.Permission.ReadMediaVideo, true));
        }
        else
        {
            result.Add((global::Android.Manifest.Permission.AccessFineLocation, true));
            result.Add((global::Android.Manifest.Permission.ReadExternalStorage, true));
        }

        requiredPermissions = result.ToArray();
    }
}
