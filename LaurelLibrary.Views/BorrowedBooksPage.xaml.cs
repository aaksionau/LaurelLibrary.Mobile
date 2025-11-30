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
        
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(BorrowedBooksViewModel.IsBusy) 
            or nameof(BorrowedBooksViewModel.HasError) 
            or nameof(BorrowedBooksViewModel.HasBorrowedBooks))
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.ForceLayout();
            });
        }
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
