﻿using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Platforms;

namespace FileShareConnectivity;

public static class Program
{
    public static MauiAppBuilder UseFileShareConnectivity(this MauiAppBuilder builder)
    {
        registerServices(builder.Services);
        registerEssentials(builder.Services);

        return builder;
    }

    private static void registerServices(in IServiceCollection services)
    {
        // TODO: Make more sence to register as AddTransient instead of AddSingleton because the context will be specific
        services.AddTransient<INetworkService, NetworkService>();
        //services.AddSingleton<IFileTransferService, FileTransferService>();
    }

    private static void registerEssentials(in IServiceCollection services)
    {
        services.AddSingleton<FileSharingWrapper>();
    }
}