using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaurelLibrary.Core.Models;
using LaurelLibrary.Core.Services;
using System.Collections.ObjectModel;

namespace LaurelLibrary.Core.ViewModels;

public partial class ReturnBooksViewModel : ObservableObject
{
    private readonly IReaderService _readerService;

    [ObservableProperty]
    private ObservableCollection<BorrowedBookDto> borrowedBooks = new();

    [ObservableProperty]
    private bool isScanning = true;

    [ObservableProperty]
    private string scannedIsbn = string.Empty;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isProcessing;

    [ObservableProperty]
    private BorrowedBookDto? selectedBook;

    [ObservableProperty]
    private bool showBookDetails;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isLoadingDetails;

    [ObservableProperty]
    private string scanningStatus = "Ready to scan";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);
    public bool HasBorrowedBooks => BorrowedBooks.Count > 0;
    public bool ShowEmptyState => !HasBorrowedBooks && !ShowBookDetails && !IsLoadingDetails;

    public ReturnBooksViewModel(IReaderService readerService)
    {
        _readerService = readerService;
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
                BorrowedBooks = new ObservableCollection<BorrowedBookDto>(readerInfo.BorrowedBooks);
                if (!HasBorrowedBooks)
                {
                    ErrorMessage = "No borrowed books found";
                }
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

    public async Task ProcessScannedIsbn(string isbn)
    {
        ClearMessages();
        IsScanning = false;
        IsLoadingDetails = true;
        ScanningStatus = "Processing...";

        // Simulate brief processing delay for better UX feedback
        await Task.Delay(300);

        var book = FindBookByIsbn(isbn);

        if (book == null)
        {
            ErrorMessage = $"Book with ISBN {isbn} not found in your borrowed books.";
            ShowBookDetails = false;
            IsLoadingDetails = false;
            ScanningStatus = "Ready to scan";
            IsScanning = true;
            return;
        }

        SelectedBook = book;
        ShowBookDetails = true;
        IsLoadingDetails = false;
    }

    private BorrowedBookDto? FindBookByIsbn(string isbn)
    {
        return BorrowedBooks.FirstOrDefault(b => b.Isbn?.Equals(isbn, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    [RelayCommand]
    public async Task ConfirmReturn()
    {
        if (SelectedBook == null || string.IsNullOrEmpty(SelectedBook.Isbn))
        {
            ErrorMessage = "No book selected to return.";
            return;
        }

        IsProcessing = true;
        ClearMessages();

        try
        {
            var readerIdStr = await SecureStorage.GetAsync("reader_id");
            var libraryIdStr = await SecureStorage.GetAsync("library_id");

            if (string.IsNullOrEmpty(readerIdStr) || !Int32.TryParse(readerIdStr, out var readerId))
            {
                ErrorMessage = "Unable to load reader information";
                IsProcessing = false;
                return;
            }

            if (string.IsNullOrEmpty(libraryIdStr) || !Guid.TryParse(libraryIdStr, out var libraryId))
            {
                ErrorMessage = "Unable to load library information";
                IsProcessing = false;
                return;
            }

            var success = await _readerService.ReturnBookRequestAsync(readerId, libraryId, SelectedBook.BookInstanceId);

            if (success)
            {
                SuccessMessage = $"Return request sent for '{SelectedBook.Title}'";
                BorrowedBooks.Remove(SelectedBook);
                SelectedBook = null;
                ShowBookDetails = false;
                ScannedIsbn = string.Empty;
                
                await Task.Delay(2000);
                IsScanning = true;
                ScanningStatus = "Ready to scan";
            }
            else
            {
                ErrorMessage = "Failed to submit return request. Please try again.";
                IsScanning = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            IsScanning = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    public void CancelReturn()
    {
        SelectedBook = null;
        ShowBookDetails = false;
        ScannedIsbn = string.Empty;
        ClearMessages();
        IsLoadingDetails = false;
        ScanningStatus = "Ready to scan";
        IsScanning = true;
    }

    private void ClearMessages()
    {
        SuccessMessage = string.Empty;
        ErrorMessage = string.Empty;
    }

    partial void OnShowBookDetailsChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    partial void OnIsLoadingDetailsChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    partial void OnBorrowedBooksChanged(ObservableCollection<BorrowedBookDto> value)
    {
        OnPropertyChanged(nameof(HasBorrowedBooks));
        OnPropertyChanged(nameof(ShowEmptyState));
    }
}
