using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaurelLibrary.Core.Models;
using LaurelLibrary.Core.Services;
using System.Collections.ObjectModel;

namespace LaurelLibrary.Core.ViewModels;

public partial class BorrowedBooksViewModel : ObservableObject
{
    private readonly IReaderService _readerService;

    [ObservableProperty]
    private ObservableCollection<BorrowedBookDto> borrowedBooks = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string readerName = string.Empty;

    public bool IsNotBusy => !IsBusy;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasBorrowedBooks => BorrowedBooks.Count > 0;

    public BorrowedBooksViewModel(IReaderService readerService)
    {
        _readerService = readerService;
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    partial void OnBorrowedBooksChanged(ObservableCollection<BorrowedBookDto> value)
    {
        OnPropertyChanged(nameof(HasBorrowedBooks));
    }

    [RelayCommand]
    public async Task LoadBorrowedBooks()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var readerIdStr = await SecureStorage.GetAsync("reader_id");
            var libraryIdStr = await SecureStorage.GetAsync("library_id");

            if (string.IsNullOrEmpty(readerIdStr) || !Int32.TryParse(readerIdStr, out var readerId))
            {
                ErrorMessage = "Unable to load reader information";
                return;
            }

            if (string.IsNullOrEmpty(libraryIdStr) || !Guid.TryParse(libraryIdStr, out var libraryId))
            {
                ErrorMessage = "Unable to load library information";
                return;
            }

            var readerInfo = await _readerService.GetReaderInfoAsync(readerId, libraryId);

            if (readerInfo != null)
            {
                ReaderName = readerInfo.Name ?? "Reader";
                BorrowedBooks = new ObservableCollection<BorrowedBookDto>(readerInfo.BorrowedBooks);
            }
            else
            {
                ErrorMessage = "Failed to load borrowed books";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading books: {ex.Message}";
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
}
