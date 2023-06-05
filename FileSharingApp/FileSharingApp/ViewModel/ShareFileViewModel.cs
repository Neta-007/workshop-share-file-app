using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using FileSharingApp.Exceptions;
using FileShareConnectivity.Models;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity;
using FileSharingApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileSharingApp.ViewModel;

/*
 * TODO: Maybe show some indication in the view for the `FilePath`?
 */
public partial class ShareFileViewModel : ObservableObject
{
    private FileSharingWrapper _fileSharingWrapper;
    private IPermissionsService _permissionsService;
    private readonly ILogger<ShareFileViewModel> _logger;

    public string FilePath { get; set; }

    [ObservableProperty]
    private bool isScaningProcessViewShouldBeActive;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotScanningForDevices))]
    private bool isScanningForDevices;

    [ObservableProperty]
    private ObservableCollection<NearbyDevice> nearbyDevices = new();

    public bool IsNotScanningForDevices => !IsScanningForDevices;

    public ShareFileViewModel(ILogger<ShareFileViewModel> logger, FileSharingWrapper fileSharingWrapper, IPermissionsService permissionsService)
    {
        _logger = logger;
        _permissionsService = permissionsService;
        _fileSharingWrapper = fileSharingWrapper;
        _fileSharingWrapper.NetworkService.ScanStateChanged += NetworkService_ScanStateChanged;
        _fileSharingWrapper.NetworkService.DevicesFound += NetworkService_DevicesFound;
        _fileSharingWrapper.NetworkService.ConnectionResult += NetworkService_ConnectionResult;
    }

    [RelayCommand]
    void DeviceFrameClicked(NearbyDevice deviceObject)
    {
        try
        {
            _logger.LogDebug($"Device frame clicked, deviceObject is: {deviceObject}");
            _fileSharingWrapper.NetworkService.EstablishConnection(deviceObject);
        }
        catch(MyException ex)
        {
            _logger.LogError($"DeviceFrameClicked {ex}. deviceObject is: {deviceObject}.");
            App.AlertService.ShowAlert(ex.Title, ex.Message);
        }
    }

    [RelayCommand]
    void FindDevices()
    {
        try
        {
            requestPermission();
            if (!IsScanningForDevices)
            {
                _logger.LogDebug($"FindDevices is starting");
                _fileSharingWrapper.NetworkService.StartDiscoverNearbyDevices();
            }
        }
        catch (MyException ex)
        {
            resetView();
            _logger.LogError($"FindDevices {ex}.");
            App.AlertService.ShowAlert(ex.Title, ex.Message);
        } 
    }

    [RelayCommand]
    public async void PickAndShow()
    {
        try
        {
            FileResult result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a File"
            });

            if (result != null)
            {
                FilePath = result.FullPath;
                _logger.LogDebug($"PickAndShow FilePath: {FilePath}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"PickAndShow The user canceled or something went wrong. {ex}.");
        }
    }
    
    private void NetworkService_DevicesFound(object sender, DevicesEventArgs e)
    {
        /*
         * TODO: The device list update is not working if we come from an intent. :(
         * This is a bug in .NET, resolved in .NET 8. 
         * More details about it here: https://github.com/dotnet/maui/issues/12219"
         */

        Task.Run(() =>
        {
            // Application.Current.Dispatcher.Dispatch
            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                // Clear the NearbyDevices collection manually, There is a bug when using "NearbyDevices.Clear();" 
                // https://github.com/dotnet/maui/issues/12219
                _logger.LogDebug($"NetworkService_DevicesFound is starting. Found {NearbyDevices.Count} devices");
                while (NearbyDevices.Count > 0)
                {
                    NearbyDevices.RemoveAt(0);
                }

                if (e.DeviceList?.Any() == true)
                {
                    foreach (NearbyDevice device in e.DeviceList)
                    {
                        NearbyDevices.Add(device);
                    }
                }
                else
                {
                    App.AlertService.ShowAlert("", "No device found");
                }

                _logger.LogDebug($"NetworkService_DevicesFound is finish");
            });
        });
    }

    private void refreshScanIndicators(bool isScanStarted)
    {
        // TODO: Validate that it's working on the Galaxy device. I think it's still not refreshing.

        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsScanningForDevices = isScanStarted;
            IsScaningProcessViewShouldBeActive = isScanStarted;
        });
    }

    private void resetView()
    {
        // TODO: Validate that it's working on the Galaxy device. I think it's still not refreshing.

        MainThread.BeginInvokeOnMainThread(() =>
        {
            NearbyDevices.Clear();
            refreshScanIndicators(false);
        });
    }

    private void NetworkService_ScanStateChanged(object sender, ScanStateEventArgs e)
    {
        switch(e.eScanState)
        {
            case FileShareConnectivity.enums.ScanState.Reset:
            case FileShareConnectivity.enums.ScanState.Failed:
                resetView();
                break;
            case FileShareConnectivity.enums.ScanState.Started:
                refreshScanIndicators(true);
                break;
            case FileShareConnectivity.enums.ScanState.Stopped:
                refreshScanIndicators(false);
                break;
        }
    }

    private void NetworkService_ConnectionResult(object sender, ConnectionResultEventArgs e)
    {
        if(e.IsSuccessConnection && e.ConnectionInfo != null)
        {
            // FilePath != null => for the sender, FilePath = file picker/share activity
            _logger.LogDebug($"NetworkService_ConnectionResult is finish {e.ConnectionInfo}. File path: {FilePath}");
            _fileSharingWrapper.FileTransferService.StartFileTransfer(e.ConnectionInfo, FilePath);
        }
    }

    private async void requestPermission()
    {
        PermissionStatus permissionStatus = await _permissionsService.CheckPermissionsAsync();
        _logger.LogDebug($"requestPermission permission status is {permissionStatus}.");
        if (permissionStatus != PermissionStatus.Granted)
        {
            permissionStatus = await _permissionsService.RequestPermissionsAsync();
            if (permissionStatus != PermissionStatus.Granted)
            {
                throw new MyException("Permissions", "Permissions are not granted.");
            }
        }
    }
}
