﻿using CommunityToolkit.Mvvm.ComponentModel;
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

    public string FilePath { get; set; }

    [ObservableProperty]
    private bool isScaningProcessViewShouldBeActive;    // TODO: check if we need it

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotScanningForDevices))]
    private bool isScanningForDevices;

    [ObservableProperty]
    private ObservableCollection<NearbyDevice> nearbyDevices = new();

    public bool IsNotScanningForDevices => !IsScanningForDevices;

    public ShareFileViewModel(FileSharingWrapper fileSharingWrapper, IPermissionsService permissionsService)
    {
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
            _fileSharingWrapper.NetworkService.EstablishConnection(deviceObject);
        }
        catch(MyException ex)
        {
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
                _fileSharingWrapper.NetworkService.StartDiscoverNearbyDevices();
            }
        }
        catch (MyException ex)
        {
            resetView();
            App.AlertService.ShowAlert(ex.Title, ex.Message);
        } 
    }

    private void NetworkService_DevicesFound(object sender, DevicesEventArgs e)
    {
        NearbyDevices.Clear();
        if (e.DeviceList?.Any() == true)        // The collection is not null and contains at least one item
        {
            foreach (NearbyDevice device in e.DeviceList)
            {
                NearbyDevices.Add(device);
            }
        }
        else
        {
            App.AlertService.ShowAlert("", "No device found");    // change to toast msg?
        }
    }

    private void refreshScanIndicators(bool isScanStarted)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsScanningForDevices = isScanStarted;
            IsScaningProcessViewShouldBeActive = isScanStarted;
        });
    }

    private void resetView()
    {
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
            // mark the device frame with green color?...

            if (FilePath != null)
            {
                _fileSharingWrapper.FileTransferService.SendFileAsync(e.ConnectionInfo, FilePath);
            }
            else
            {
                // from file picker?
            }
        }
    }

    private async void requestPermission()
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
