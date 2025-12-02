using Microsoft.Extensions.Logging;
using UraniumUI;
using LaurelLibrary.Core.Services;
using LaurelLibrary.Core.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace LaurelLibrary.Views;

public static class MauiProgram
{
	// Configuration constants
	private const string BlobStorageDomain = "https://mylibrarianprodza8twn.blob.core.windows.net";
	private const string BaseApiUrl = "https://mylibrarian.org";

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.UseBarcodeReader()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFontAwesomeIconFonts();
			});

		// Register HttpClient with configuration for Android
		builder.Services.AddHttpClient("default", client =>
		{
			client.Timeout = TimeSpan.FromSeconds(30);
		})
#if ANDROID
		.ConfigurePrimaryHttpMessageHandler(() =>
		{
			var handler = new HttpClientHandler();
			// Allow all SSL certificates in debug mode (remove in production)
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
			return handler;
		})
#endif
		;

		// Register services
		builder.Services.AddSingleton<ILibraryService>(sp => 
		{
			var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
			var httpClient = httpClientFactory.CreateClient("default");
			return new LibraryService(httpClient, BaseApiUrl);
		});
		builder.Services.AddSingleton<IAuthenticationService>(sp => 
		{
			var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
			var httpClient = httpClientFactory.CreateClient("default");
			return new AuthenticationService(httpClient, BaseApiUrl);
		});
		builder.Services.AddSingleton<IReaderService>(sp => 
		{
			var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
			var httpClient = httpClientFactory.CreateClient("default");
			return new ReaderService(httpClient, BaseApiUrl, BlobStorageDomain);
		});

		// Register ViewModels
		builder.Services.AddTransient<ReaderAuthenticationViewModel>();
		builder.Services.AddTransient<LibrarySearchPageViewModel>();
		builder.Services.AddTransient<BorrowedBooksViewModel>();
		builder.Services.AddTransient<ReturnBooksViewModel>();

		// Register Pages
		builder.Services.AddTransient<ReaderAuthenticationPage>();
		builder.Services.AddTransient<LibrarySearchPage>();
		builder.Services.AddTransient<BorrowedBooksPage>();
		builder.Services.AddTransient<ReturnBooksPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
