using LaurelLibrary.Core.ViewModels;

namespace LaurelLibrary.Views;

public partial class BorrowedBooksPage : ContentPage
{
    private BorrowedBooksViewModel? _viewModel;

    public BorrowedBooksPage(BorrowedBooksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.LoadBorrowedBooks();
        }
    }
}
