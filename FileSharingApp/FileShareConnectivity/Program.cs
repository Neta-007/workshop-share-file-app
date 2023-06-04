using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Platforms;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FileShareConnectivity;

public static class Program
{
    public static MauiAppBuilder UseFileShareConnectivity(this MauiAppBuilder builder)
    {
        registerServices(builder.Services);
        registerEssentials(builder.Services);
        registerLogger(builder.Services);

        return builder;
    }

    private static void registerServices(in IServiceCollection services)
    {
        services.AddSingleton<INetworkService, NetworkService>();
        services.AddSingleton<IFileTransferService, FileTransferService>();
        services.AddSingleton<ILoggerProvider, LoggerProvider>();
    }

    private static void registerEssentials(in IServiceCollection services)
    {
        services.AddSingleton<FileSharingWrapper>();
    }

    private static void registerLogger(in IServiceCollection services)
    {
        // Get the assembly's full name
        string assemblyFullName = Assembly.GetEntryAssembly()?.FullName;    // TODO: return null

        services.AddLogging(configure =>
        {
#if ANDROID
#if DEBUG
            LogLevel androidLogLevel = LogLevel.Debug;
#else
            LogLevel androidLogLevel = LogLevel.Information;
#endif

            configure.AddProvider(new LoggerProvider())
                        .AddFilter(assemblyFullName, androidLogLevel);
#endif
        });
    }
}