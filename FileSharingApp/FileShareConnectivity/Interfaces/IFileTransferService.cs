
namespace FileShareConnectivity.Interfaces;

internal interface IFileTransferService
{
    //Task<bool> SendNotificationAsync(string deviceId, string message);
    Task<bool> SendFileAsync();// (string deviceId, FileModel file);
    //Task<FileModel> ReceiveFileAsync();
}
