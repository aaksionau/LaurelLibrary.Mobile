using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaurelLibrary.Core.Models;
using LaurelLibrary.Core.Services;
using System.Collections.Generic;

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

    public bool IsNotBusy => !IsBusy;
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

        await Shell.Current.GoToAsync($"//librarySearchPage?query={Uri.EscapeDataString(LibrarySearchQuery)}");
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
                // Set the user role to Reader
                Preferences.Set("UserRole", "Reader");

                // Navigate to reader tabs (borrowed books page)
                await Shell.Current.GoToAsync("//readerTabs/borrowedBooksPage");
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
        await Shell.Current.GoToAsync("//authenticationPage");
    }

    public async Task ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query != null && query.TryGetValue("selectedLibraryId", out var libraryId))
        {
            if (Guid.TryParse(libraryId.ToString(), out var id))
            {
                var libraryName = query.TryGetValue("selectedLibraryName", out var name) ? Uri.UnescapeDataString(name.ToString() ?? "") : "";
                var libraryAddress = query.TryGetValue("selectedLibraryAddress", out var address) ? Uri.UnescapeDataString(address.ToString() ?? "") : "";

                // Create a library object from the parameters
                SelectedLibrary = new LibraryDto { Id = id, Name = libraryName, Address = libraryAddress };
                ErrorMessage = string.Empty;
            }
        }
    }
}
