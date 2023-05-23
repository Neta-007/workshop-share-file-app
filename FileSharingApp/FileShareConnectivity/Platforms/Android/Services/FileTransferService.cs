using Android.App;
using AndroidX.Annotations;
using FileShareConnectivity.Interfaces;
using Java.Lang;

namespace FileShareConnectivity.Platforms;

internal class FileTransferService: AndroidX.Arch.Core.Executor.TaskExecutor, IFileTransferService
{
    public Task<bool> SendFileAsync()
    {
        throw new NotImplementedException();
    }

    public override void ExecuteOnDiskIO(IRunnable p0)
    {
        throw new NotImplementedException();
    }

    public override void PostToMainThread(IRunnable p0)
    {
        throw new NotImplementedException();
    }

    public override bool IsMainThread { get; }
}