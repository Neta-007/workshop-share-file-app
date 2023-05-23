
namespace FileSharingApp.Interfaces;

public interface IPermissionsService
{
    Task<PermissionStatus> CheckPermissionsAsync();
    Task<PermissionStatus> RequestPermissionsAsync();
}
