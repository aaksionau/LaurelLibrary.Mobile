namespace LaurelLibrary.Views;

public partial class AuthenticationPage : ContentPage
{
    public AuthenticationPage()
    {
        InitializeComponent();
    }

    private async void OnReaderSelected(object sender, EventArgs e)
    {
        // Navigate to reader authentication page
        await Shell.Current.GoToAsync("//readerAuthenticationPage");
    }

    private async void OnAdministratorSelected(object sender, EventArgs e)
    {

        // Store the selected role (you can use Preferences or a service)
        Preferences.Set("UserRole", "Administrator");

        // Navigate to main page
        await Shell.Current.GoToAsync("//mainPage");
    }
}
