namespace LaurelLibrary.Views;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Navigate to authentication page if no user role is set
		if (!Preferences.ContainsKey("UserRole"))
		{
			Loaded += async (s, e) => await GoToAsync("//authenticationPage");
		}
		else
		{
			Loaded += async (s, e) => await GoToAsync("//mainPage");
		}
	}
}
