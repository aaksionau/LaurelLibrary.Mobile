using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaurelLibrary.Core.Models;
using LaurelLibrary.Core.Services;
using System.Collections.ObjectModel;

namespace LaurelLibrary.Core.ViewModels;

public partial class ReaderAuthenticationViewModel : ObservableObject
{
    private readonly ILibraryService _libraryService;
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private string _librarySearchQuery = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private LibraryDto? _selectedLibrary;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LibraryDto> _libraries = new();

    public bool IsNotBusy => !IsBusy;
    public bool HasLibraries => Libraries.Count > 0;
    public bool HasSelectedLibrary => SelectedLibrary != null;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool CanAuthenticate => HasSelectedLibrary && !string.IsNullOrWhiteSpace(Email) && !IsBusy;

    public ReaderAuthenticationViewModel(
        ILibraryService libraryService,
        IAuthenticationService authenticationService)
    {
        _libraryService = libraryService;
        _authenticationService = authenticationService;
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(CanAuthenticate));
        AuthenticateCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedLibraryChanged(LibraryDto? value)
    {
        OnPropertyChanged(nameof(HasSelectedLibrary));
        OnPropertyChanged(nameof(CanAuthenticate));
    }

    partial void OnEmailChanged(string value)
    {
        OnPropertyChanged(nameof(CanAuthenticate));
    }

    partial void OnLibrariesChanged(ObservableCollection<LibraryDto> value)
    {
        OnPropertyChanged(nameof(HasLibraries));
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    [RelayCommand]
    private async Task SearchLibraries()
    {
        if (string.IsNullOrWhiteSpace(LibrarySearchQuery))
        {
            ErrorMessage = "Please enter a search query";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var results = await _libraryService.SearchLibrariesAsync(LibrarySearchQuery);
            Libraries.Clear();
            
            foreach (var library in results)
            {
                Libraries.Add(library);
            }

            if (!HasLibraries)
            {
                ErrorMessage = "No libraries found matching your search";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error searching libraries: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAuthenticate))]
    private async Task Authenticate()
    {
        if (SelectedLibrary == null || string.IsNullOrWhiteSpace(Email))
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _authenticationService.AuthenticateReaderAsync(
                Email,
                SelectedLibrary.Id);

            if (success)
            {
                // Navigate to main page
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "Authentication failed. Please check your email and try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Authentication error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("//AuthenticationPage");
    }
}
