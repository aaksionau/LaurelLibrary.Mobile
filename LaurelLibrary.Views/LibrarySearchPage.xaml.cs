using LaurelLibrary.Core.ViewModels;

namespace LaurelLibrary.Views;

public partial class LibrarySearchPage : ContentPage, IQueryAttributable
{
    private LibrarySearchPageViewModel? _viewModel;

    public LibrarySearchPage(LibrarySearchPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query != null && query.TryGetValue("query", out var searchQuery))
        {
            await _viewModel?.InitializeAsync(searchQuery.ToString() ?? "");
        }
    }
}
