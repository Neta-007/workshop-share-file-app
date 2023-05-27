using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity.Platforms;

internal class FileTransferService: IFileTransferService
{
    public Task<bool> SendFileAsync(object connectionInfo, string filePath)
    {
        throw new NotImplementedException();
    }
}