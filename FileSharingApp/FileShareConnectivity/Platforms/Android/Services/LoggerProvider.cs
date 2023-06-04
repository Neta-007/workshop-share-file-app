using FileShareConnectivity.Platforms.Android.Services;
using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms;

internal class LoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        // Category name is often the full class name, like
        // MyApp.ViewModel.MyViewModel
        // This removes the namespace:
        //int lastDotPos = categoryName.LastIndexOf('.');

        int firstDotPos = categoryName.IndexOf('.'); // project name
        if (firstDotPos > 0)
        {
            categoryName = categoryName.Substring(firstDotPos + 1);
        }

        return new AndroidLogger(categoryName);
    }

    public void Dispose() { }
}
