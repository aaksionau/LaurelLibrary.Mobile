using LaurelLibrary.ApiClient;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace LaurelLibrary.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly LaurelLibrary.ApiClient.ApiClient _apiClient;
    private string? _authToken;

    public AuthenticationService(HttpClient httpClient, string baseUrl = "http://localhost:5083")
    {
        var adapter = new HttpClientRequestAdapter(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient
        );
        adapter.BaseUrl = baseUrl;
        _apiClient = new LaurelLibrary.ApiClient.ApiClient(adapter);
    }

    public async Task<bool> AuthenticateReaderAsync(string email, Guid libraryId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create authentication request body
            var verifyRequest = new LaurelLibrary.ApiClient.Models.MobileReaderVerificationRequestDto
            {
                Email = email,
                LibraryId = libraryId
            };

            // Call the verify reader endpoint
            var response = await _apiClient.Api.Mobile.VerifyReader.PostAsync(verifyRequest, cancellationToken: cancellationToken);

            if (response?.IsVerified == true && response.Reader != null)
            {
                // Store reader info in secure storage
                await SecureStorage.SetAsync("user_email", email);
                await SecureStorage.SetAsync("library_id", libraryId.ToString());
                await SecureStorage.SetAsync("reader_id", response.Reader.ReaderId?.ToString() ?? string.Empty);
                await SecureStorage.SetAsync("is_authenticated", "true");
                await SecureStorage.SetAsync("user_role", "reader");
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            // Log error if needed
            return false;
        }
    }

    public async Task<string?> GetAuthTokenAsync()
    {
        if (!string.IsNullOrEmpty(_authToken))
            return _authToken;

        // Try to retrieve from secure storage
        _authToken = await SecureStorage.GetAsync("auth_token");
        return _authToken;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var isAuthenticated = await SecureStorage.GetAsync("is_authenticated");
        return isAuthenticated == "true";
    }

    public async Task<string?> GetUserRoleAsync()
    {
        return await SecureStorage.GetAsync("user_role");
    }

    public async Task LogoutAsync()
    {
        _authToken = null;
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("library_id");
        SecureStorage.Remove("reader_id");
        SecureStorage.Remove("is_authenticated");
        SecureStorage.Remove("user_role");
        await Task.CompletedTask;
    }
}
