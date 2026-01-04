namespace MyHomeApp.Services;

public interface IDiagnosticService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);
    Task<string> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons);
    Task<string?> ShowPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = null, int maxLength = -1, Keyboard? keyboard = null, string initialValue = "");
    Task ShowToastAsync(string message);
    Task ShowSnackbarAsync(string message, string? actionText = null, Action? action = null, int duration = 3);
}
