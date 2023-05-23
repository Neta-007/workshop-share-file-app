
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

        result.Add((global::Android.Manifest.Permission.AccessWifiState, false));
        result.Add((global::Android.Manifest.Permission.ChangeWifiState, false));
        result.Add((global::Android.Manifest.Permission.ChangeNetworkState, false));
        result.Add((global::Android.Manifest.Permission.Internet, false));
        result.Add((global::Android.Manifest.Permission.AccessNetworkState, false));

        if (sdk >= 33)
        {
            result.Add((global::Android.Manifest.Permission.NearbyWifiDevices, true));
        }
        else
        {
            result.Add((global::Android.Manifest.Permission.AccessFineLocation, true));
        }

        requiredPermissions = result.ToArray();
    }
}
