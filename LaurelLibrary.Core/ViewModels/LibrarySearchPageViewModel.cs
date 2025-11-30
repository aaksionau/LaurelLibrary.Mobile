using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaurelLibrary.Core.Models;
using LaurelLibrary.Core.Services;
using System.Collections.ObjectModel;

namespace LaurelLibrary.Core.ViewModels;

public partial class LibrarySearchPageViewModel : ObservableObject
{
    private readonly ILibraryService _libraryService;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LibraryDto> libraries = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private LibraryDto? selectedLibrary;

    public bool IsNotBusy => !IsBusy;
    public bool HasLibraries => Libraries.Count > 0;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public LibrarySearchPageViewModel(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    partial void OnLibrariesChanged(ObservableCollection<LibraryDto> value)
    {
        OnPropertyChanged(nameof(HasLibraries));
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    public async Task InitializeAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        SearchQuery = query.Trim();
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var results = await _libraryService.SearchLibrariesAsync(query);
            Libraries = new ObservableCollection<LibraryDto>(results);

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

    [RelayCommand]
    private async Task SelectLibrary(LibraryDto library)
    {
        if (library == null)
            return;

        SelectedLibrary = library;

        // Pass the selected library back using query parameters and navigate
        var navigationParameter = $"selectedLibraryId={library.Id:D}&selectedLibraryName={Uri.EscapeDataString(library.Name)}&selectedLibraryAddress={Uri.EscapeDataString(library.Address)}";
        await Shell.Current.GoToAsync($"//readerAuthenticationPage?{navigationParameter}");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("//authenticationPage");
    }
}
