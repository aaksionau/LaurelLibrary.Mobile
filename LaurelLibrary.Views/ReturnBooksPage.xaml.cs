using LaurelLibrary.Core.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace LaurelLibrary.Views;

public partial class ReturnBooksPage : ContentPage
{
    private ReturnBooksViewModel? _viewModel;

    public ReturnBooksPage(ReturnBooksViewModel viewModel)
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
            // Load borrowed books
            await _viewModel.LoadBorrowedBooksCommand.ExecuteAsync(null);
            _viewModel.IsScanning = _viewModel.HasBorrowedBooks;
            
            // Subscribe to barcode detection
            if (cameraBarcodeReaderView != null)
            {
                cameraBarcodeReaderView.Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormats.OneDimensional,
                    AutoRotate = true,
                    Multiple = true
                };
                cameraBarcodeReaderView.BarcodesDetected += CameraView_BarCodeDetectionFrameAvailable;
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_viewModel != null)
        {
            _viewModel.IsScanning = false;
        }
        
        if (cameraBarcodeReaderView != null)
        {
            cameraBarcodeReaderView.BarcodesDetected -= CameraView_BarCodeDetectionFrameAvailable;
        }
    }

    private void CameraView_BarCodeDetectionFrameAvailable(object sender, BarcodeDetectionEventArgs e)
    {
        if (_viewModel == null || !_viewModel.IsScanning)
            return;

        var barcode = e.Results?.FirstOrDefault();
        if (barcode != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.ProcessScannedIsbn(barcode.Value);
                
                // Provide haptic feedback on successful scan
                try
                {
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                catch
                {
                    // Haptic feedback not available on all devices
                }
            });
        }
    }
}
