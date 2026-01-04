namespace MyHomeApp.Services;

/// <summary>
/// Service interface for Shell navigation
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to a route
    /// </summary>
    Task GoToAsync(ShellNavigationState state);

    /// <summary>
    /// Navigate to a route with animation option
    /// </summary>
    Task GoToAsync(ShellNavigationState state, bool animate);

    /// <summary>
    /// Navigate to a route with parameters
    /// </summary>
    Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters);

    /// <summary>
    /// Navigate to a route with animation option and parameters
    /// </summary>
    Task GoToAsync(ShellNavigationState state, bool animate, IDictionary<string, object> parameters);

    /// <summary>
    /// Navigate to a route with query parameters
    /// </summary>
    Task GoToAsync(ShellNavigationState state, ShellNavigationQueryParameters shellNavigationQueryParameters);

    /// <summary>
    /// Navigate to a route with animation option and query parameters
    /// </summary>
    Task GoToAsync(ShellNavigationState state, bool animate, ShellNavigationQueryParameters shellNavigationQueryParameters);

    /// <summary>
    /// Navigate back to the previous page
    /// </summary>
    Task GoBackAsync();

    /// <summary>
    /// Navigate back with parameters
    /// </summary>
    Task GoBackAsync(IDictionary<string, object> parameters);
}
