

namespace FileShareConnectivity.Interfaces;

public interface IFileTransferService
{
    //Task<bool> SendFileAsync(object connectionInfo, string filePath);// (string deviceId, FileModel file);
    void StartFileTransfer(object connectionInfo, string filePath);
}
