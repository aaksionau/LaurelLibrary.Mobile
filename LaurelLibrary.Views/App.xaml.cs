using LaurelLibrary.Core.Services;

namespace LaurelLibrary.Views;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await CheckAndNavigateBasedOnAuthentication();
	}

	private async Task CheckAndNavigateBasedOnAuthentication()
	{
		try
		{
			// Get the authentication service from the service provider
			if (Application.Current?.Handler?.MauiContext?.Services is not null)
			{
				var authService = Application.Current.Handler.MauiContext.Services.GetService<IAuthenticationService>();
				
				if (authService != null)
				{
					var isAuthenticated = await authService.IsAuthenticatedAsync();
					var userRole = await authService.GetUserRoleAsync();

					if (isAuthenticated && userRole == "reader")
					{
						// Redirect reader role users to reader tabs with borrowed books page
						await Shell.Current.GoToAsync("//readerTabs/borrowedBooksPage");
					}
				}
			}
		}
		catch (Exception ex)
		{
			// Log error if needed
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
		}
	}
}