using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MyHomeApp.Services;

/// <summary>
/// Implementation of diagnostic service for alerts and notifications
/// </summary>
public sealed class DiagnosticService : IDiagnosticService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK") => Shell.Current.DisplayAlertAsync(title, message, cancel);

    public Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel) => Shell.Current.DisplayAlertAsync(title, message, accept, cancel);

    public Task<string> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons) => Shell.Current.DisplayActionSheetAsync(title, cancel, destruction, buttons);

    public Task<string?> ShowPromptAsync(
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel",
        string? placeholder = null,
        int maxLength = -1,
        Keyboard? keyboard = null,
        string initialValue = "") => Shell.Current.DisplayPromptAsync(
            title,
            message,
            accept,
            cancel,
            placeholder,
            maxLength,
            keyboard,
            initialValue);

    public async Task ShowToastAsync(string message)
    {
        var toast = Toast.Make(message, ToastDuration.Short, 14);
        await toast.Show();
    }

    public async Task ShowSnackbarAsync(string message, string? actionText = null, Action? action = null, int duration = 3)
    {
        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.DarkSlateGray,
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.LightGreen,
            CornerRadius = new CornerRadius(10),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14, FontWeight.Bold),
            CharacterSpacing = 0.5
        };

        var snackbar = action != null && actionText != null
            ? Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(duration), snackbarOptions)
            : Snackbar.Make(message, () => { }, string.Empty, TimeSpan.FromSeconds(duration), snackbarOptions);

        await snackbar.Show();
    }
}
