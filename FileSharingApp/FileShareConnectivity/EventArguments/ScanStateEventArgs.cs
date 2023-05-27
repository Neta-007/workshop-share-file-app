
using FileShareConnectivity.enums;

namespace FileShareConnectivity.EventArguments;

public class ScanStateEventArgs : EventArgs
{
    public ScanState eScanState { get; private set; }

    public ScanStateEventArgs(ScanState scanState)
    {
        eScanState = scanState;
    }
}
