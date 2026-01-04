namespace MyHomeApp.Services;

/// <summary>
/// Implementation of Shell navigation service
/// </summary>
public sealed class NavigationService : INavigationService
{
    public Task GoToAsync(ShellNavigationState state)
    {
        return Shell.Current.GoToAsync(state, true);
    }

    public Task GoToAsync(ShellNavigationState state, bool animate)
    {
        return Shell.Current.GoToAsync(state, animate);
    }

    public Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(state, true, parameters);
    }

    public Task GoToAsync(ShellNavigationState state, bool animate, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(state, animate, parameters);
    }

    public Task GoToAsync(ShellNavigationState state, ShellNavigationQueryParameters shellNavigationQueryParameters)
    {
        return Shell.Current.GoToAsync(state, true, shellNavigationQueryParameters);
    }

    public Task GoToAsync(ShellNavigationState state, bool animate, ShellNavigationQueryParameters shellNavigationQueryParameters)
    {
        return Shell.Current.GoToAsync(state, animate, shellNavigationQueryParameters);
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..", true);
    }

    public Task GoBackAsync(IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync("..", parameters);
    }
}
