using FileSharingApp.Interfaces;

namespace FileSharingApp.Platforms;

internal class PermissionsService : IPermissionsService
{
    public Task<PermissionStatus> CheckPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PermissionStatus> RequestPermissionsAsync()
    {
        throw new NotImplementedException();
    }
}
