using LaurelLibrary.Core.ViewModels;

namespace LaurelLibrary.Views;

public partial class ReaderAuthenticationPage : ContentPage, IQueryAttributable
{
    private ReaderAuthenticationViewModel? _viewModel;

    public ReaderAuthenticationPage(ReaderAuthenticationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await (_viewModel?.ApplyQueryAttributes(query) ?? Task.CompletedTask);
    }
}
