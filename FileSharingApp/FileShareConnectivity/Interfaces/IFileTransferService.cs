
using FileShareConnectivity.Models;

namespace FileShareConnectivity.Interfaces;

public interface IFileTransferService
{
    //Task<bool> SendNotificationAsync(string deviceId, string message);
    Task<bool> SendFileAsync(object connectionInfo, string filePath);// (string deviceId, FileModel file);
    //Task<FileModel> ReceiveFileAsync();
}
