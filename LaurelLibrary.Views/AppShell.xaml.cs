namespace LaurelLibrary.Views;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Navigate to authentication page if no user role is set
		if (!Preferences.ContainsKey("UserRole"))
		{
			Loaded += async (s, e) => await GoToAsync("//AuthenticationPage");
		}
		else
		{
			Loaded += async (s, e) => await GoToAsync("//MainPage");
		}
	}
}
