using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity.Platforms;

internal class FileTransferService: IFileTransferService
{
    public Task<bool> SendFileAsync()
    {
        throw new NotImplementedException();
    }
}