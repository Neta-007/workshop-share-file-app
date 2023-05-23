using FileSharingApp.Exceptions;
using FileSharingApp.Interfaces;
using FileSharingApp.Platforms.Android.Permissions;

namespace FileSharingApp.Platforms;

internal class PermissionsService : IPermissionsService
{
    private static WifiPermissions wifiPermissions = new();

    public async Task<PermissionStatus> CheckPermissionsAsync()
    {
        PermissionStatus status = PermissionStatus.Denied;

        try
        {
            status = await wifiPermissions.CheckStatusAsync();
        }
        catch (Exception ex)
        {
            throw new MyException("Unable to check permissions", $"{ex.Message}.");
        }

        return status;
    }

    public async Task<PermissionStatus> RequestPermissionsAsync()
    {
        PermissionStatus status = PermissionStatus.Denied;

        try
        {
            status = await wifiPermissions.RequestAsync();
        }
        catch (Exception ex)
        {
            throw new MyException("Unable to check permissions", $"{ex.Message}.");
        }

        return status;
    }
}
