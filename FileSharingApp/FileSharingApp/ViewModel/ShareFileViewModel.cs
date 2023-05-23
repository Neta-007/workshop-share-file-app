using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using FileSharingApp.Exceptions;
using FileShareConnectivity.Models;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity;
using FileSharingApp.Interfaces;

namespace FileSharingApp.ViewModel;

public partial class ShareFileViewModel : ObservableObject
{
    private FileSharingWrapper _fileSharingWrapper;
    private IPermissionsService _permissionsService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsScaningProcessViewShouldBeDeactive))]
    private bool isScaningProcessViewShouldBeActive;    // TODO: check if we need it

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotScanningForDevices))]
    private bool isScanningForDevices;

    [ObservableProperty]
    private ObservableCollection<NearbyDevice> nearbyDevices = new();

    public bool IsNotScanningForDevices => !IsScanningForDevices;

    public bool IsScaningProcessViewShouldBeDeactive => !IsScaningProcessViewShouldBeActive;    // TODO: check if we need it

    public ShareFileViewModel(FileSharingWrapper fileSharingWrapper, IPermissionsService permissionsService)
    {
        _permissionsService = permissionsService;
        _fileSharingWrapper = fileSharingWrapper;
        _fileSharingWrapper.NetworkService.FinishScan += NetworkService_FinishScan;
        _fileSharingWrapper.NetworkService.DevicesFound += NetworkService_DevicesFound;
    }

    [RelayCommand]
    void DeviceFrameClicked(NearbyDevice deviceObject)
    {
        _fileSharingWrapper.NetworkService.EstablishConnection(deviceObject);
    }

    [RelayCommand]
    void FindDevices()
    {
        try
        {
            // If we are not wrapping the call in a Task.Run block to execute it on a background thread
            // the visual element does not reset !!!
            // hence the button for scaning devices will not disabled.
            // https://stackoverflow.com/questions/73738176/button-greyed-when-using-net-maui-community-toolkit-mvvm
            var t = Task.Run(requestPermissionAsync);
            t.Wait();

            if (!IsScanningForDevices)
            {
                NearbyDevices.Clear();
                IsScaningProcessViewShouldBeActive = true;
                IsScanningForDevices = true;
                _fileSharingWrapper.NetworkService.StartDiscoverNearbyDevices();
            }
        }
        catch (MyException ex)
        {
            App.AlertService.ShowAlert(ex.Title, ex.Message);
            NetworkService_FinishScan(null, null);      // TODO: chagne the null ?
        } 
    }

    private void NetworkService_DevicesFound(object sender, DevicesEventArgs e)
    {
        if (e.DeviceList?.Any() == true)        // The collection is not null and contains at least one item
        {
            // IsScaningProcessViewShouldBeActive = false;
            foreach (NearbyDevice device in e.DeviceList)
            {
                NearbyDevices.Add(device);
            }
        }
        else
        {
            // what??
            //string msgTitle = 
            //App.AlertService.ShowAlert("No New?", ex.Message);
        }
    }

    /*
     * A cleanup method after the scan has finished, whether the scan succeeded or failed due to an exception
     */
    private void NetworkService_FinishScan(object sender, EventArgs e)
    {
        IsScanningForDevices = false;
        IsScaningProcessViewShouldBeActive = false;
    }

    private async Task requestPermissionAsync()
    {
        PermissionStatus permissionStatus = await _permissionsService.CheckPermissionsAsync();

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
