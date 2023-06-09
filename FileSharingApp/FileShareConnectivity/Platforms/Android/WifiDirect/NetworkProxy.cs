﻿using Android.Content;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Exceptions;
using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Models;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

internal class NetworkProxy : INetworkService, IDisposable
{
    private NetworkService _networkService;

    public NetworkProxy(Context context)
    {
        _networkService = (NetworkService)MauiApplication.Current.Services.GetService(typeof(INetworkService));
        _networkService.InitNetworkServiceComponents(context);
        RegisterReceiver();
    }

    public event EventHandler<DevicesEventArgs> DevicesFound
    {
        add { _networkService.DevicesFound += value; }
        remove { _networkService.DevicesFound -= value; }
    }

    public event EventHandler<EventArgs> FinishScan
    {
        add { _networkService.FinishScan += value; }
        remove { _networkService.FinishScan -= value; }
    }

    public void RegisterReceiver()
    {
        _networkService.RegisterReceiver();
    }

    public void UnregisterReceiver()
    {
        _networkService.UnregisterReceiver();
    }

    public void StartDiscoverNearbyDevices()
    {
        validateThatWifiP2PEnabled();
        _networkService.StartDiscoverNearbyDevices();
    }

    public IEnumerable<NearbyDevice> DiscoverNearbyDevices()
    {
        validateThatWifiP2PEnabled();
        return _networkService.DiscoverNearbyDevices();
    }

    public bool EstablishConnection(NearbyDevice device)
    {
        validateThatWifiP2PEnabled();
        return _networkService.EstablishConnection(device);
    }

    public bool DisconnectFromDevice(NearbyDevice device)
    {
        // validateThatWifiP2PEnabled(); ????
        return _networkService.DisconnectFromDevice(device);
    }

    private void validateThatWifiP2PEnabled()
    {
        if (!Utils.IsWifiEnabled())
        {
            throw new MyException("Please enable Wi-Fi", "");
        }
        else if (!Utils.IsWifiDirectSupported())
        {
            throw new MyException("Wi-Fi Direct is not supported on this device", "");
        }
    }

    public void Dispose()
    {
        _networkService.UnregisterReceiver();
        _networkService.Dispose();
    }
}
