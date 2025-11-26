namespace LaurelLibrary.Core.Services;

public interface IAuthenticationService
{
    Task<bool> AuthenticateReaderAsync(string email, Guid libraryId, CancellationToken cancellationToken = default);
    Task<string?> GetAuthTokenAsync();
    Task LogoutAsync();
}
