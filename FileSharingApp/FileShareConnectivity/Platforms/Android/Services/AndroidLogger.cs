using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms.Android.Services;

internal class AndroidLogger : ILogger
{
    private readonly string _categoryName;

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public AndroidLogger(string category)
    {
        _categoryName = category;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        //string dateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");
        //string logMessage = $"{dateTime} [{_categoryName}] {formatter(state, exception)}";
        string logMessage = $"[{_categoryName}] {formatter(state, exception)}";
        Java.Lang.Throwable? throwable = exception is not null ? Java.Lang.Throwable.FromException(exception) : null;

        switch (logLevel)
        {
            case LogLevel.Trace:
                global::Android.Util.Log.Verbose(_categoryName, throwable, logMessage);
                break;
            case LogLevel.Debug:
                global::Android.Util.Log.Debug(_categoryName, throwable, logMessage);
                break;
            case LogLevel.Information:
                global::Android.Util.Log.Info(_categoryName, throwable, logMessage);
                break;
            case LogLevel.Warning:
                global::Android.Util.Log.Warn(_categoryName, throwable, logMessage);
                break;
            case LogLevel.Error:
                global::Android.Util.Log.Error(_categoryName, throwable, logMessage);
                break;
            case LogLevel.Critical:
                global::Android.Util.Log.Wtf(_categoryName, throwable, logMessage);
                break;
        }
    }
}
