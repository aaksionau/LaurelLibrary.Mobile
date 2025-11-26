using LaurelLibrary.Core.ViewModels;

namespace LaurelLibrary.Views;

public partial class ReaderAuthenticationPage : ContentPage
{
    public ReaderAuthenticationPage(ReaderAuthenticationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
