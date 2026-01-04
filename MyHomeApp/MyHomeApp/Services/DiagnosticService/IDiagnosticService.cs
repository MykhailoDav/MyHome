namespace MyHomeApp.Services;

/// <summary>
/// Service interface for displaying alerts, toasts, and diagnostic messages
/// </summary>
public interface IDiagnosticService
{
    /// <summary>
    /// Displays a simple alert dialog
    /// </summary>
    /// <param name="title">Alert title</param>
    /// <param name="message">Alert message</param>
    /// <param name="cancel">Cancel button text (default: "OK")</param>
    Task ShowAlertAsync(string title, string message, string cancel = "OK");

    /// <summary>
    /// Displays a confirmation dialog with two buttons
    /// </summary>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="accept">Accept button text</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>True if user clicked accept, false if cancelled</returns>
    Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);

    /// <summary>
    /// Displays an action sheet with multiple options
    /// </summary>
    /// <param name="title">Action sheet title</param>
    /// <param name="cancel">Cancel button text</param>
    /// <param name="destruction">Destruction button text (optional)</param>
    /// <param name="buttons">Array of button texts</param>
    /// <returns>The text of the selected button</returns>
    Task<string> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons);

    /// <summary>
    /// Displays a prompt dialog for user input
    /// </summary>
    /// <param name="title">Prompt title</param>
    /// <param name="message">Prompt message</param>
    /// <param name="accept">Accept button text</param>
    /// <param name="cancel">Cancel button text</param>
    /// <param name="placeholder">Input placeholder text</param>
    /// <param name="maxLength">Maximum input length</param>
    /// <param name="keyboard">Keyboard type</param>
    /// <param name="initialValue">Initial input value</param>
    /// <returns>The user's input, or null if cancelled</returns>
    Task<string?> ShowPromptAsync(
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel",
        string? placeholder = null,
        int maxLength = -1,
        Keyboard? keyboard = null,
        string initialValue = "");

    /// <summary>
    /// Displays a toast notification (short duration)
    /// </summary>
    /// <param name="message">Toast message</param>
    Task ShowToastAsync(string message);

    /// <summary>
    /// Displays a snackbar notification with optional action
    /// </summary>
    /// <param name="message">Snackbar message</param>
    /// <param name="actionText">Action button text (optional)</param>
    /// <param name="action">Action to execute when button is clicked (optional)</param>
    /// <param name="duration">Display duration in seconds</param>
    Task ShowSnackbarAsync(string message, string? actionText = null, Action? action = null, int duration = 3);
}
