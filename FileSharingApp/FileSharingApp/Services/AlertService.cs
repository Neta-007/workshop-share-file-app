using FileSharingApp.Interfaces;
using Microsoft.Extensions.Logging;
// https://stackoverflow.com/questions/72429055/how-to-displayalert-in-a-net-maui-viewmodel
namespace FileSharingApp.Services;

internal class AlertService : IAlertService
{
    private readonly ILogger<AlertService> _logger;

    public AlertService(ILogger<AlertService> logger)
    {
        _logger = logger;
    }
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----

    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        _logger.LogDebug($"ShowAlertAsync, Title: {title}, Message: {message}");
        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        _logger.LogDebug($"ShowConfirmationAsync, Title: {title}, Message: {message}");
        return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }


    // ----- "Fire and forget" calls -----

    /*
     * TODO:
     * Need to learn how to use it (Dispatcher)
     */

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    /// <param name="callback">Action to perform afterwards.</param>
    public void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Yes", string cancel = "No")
    {
        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
        {
            bool answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        });
    }
}
