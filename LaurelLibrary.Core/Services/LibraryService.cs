using LaurelLibrary.ApiClient;
using LaurelLibrary.Core.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace LaurelLibrary.Core.Services;

public class LibraryService : ILibraryService
{
    private readonly LaurelLibrary.ApiClient.ApiClient _apiClient;

    public LibraryService(HttpClient httpClient, string baseUrl = "https://mylibrarian.org")
    {
        var adapter = new HttpClientRequestAdapter(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient
        );
        adapter.BaseUrl = baseUrl;
        _apiClient = new LaurelLibrary.ApiClient.ApiClient(adapter);
    }

    public async Task<IEnumerable<LibraryDto>> SearchLibrariesAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchRequest = new LaurelLibrary.ApiClient.Models.MobileLibrarySearchRequestDto
            {
                SearchTerm = query
            };

            var libraries = await _apiClient.Api.Mobile.Libraries.Search.PostAsync(searchRequest, cancellationToken: cancellationToken);

            if (libraries == null)
                return Enumerable.Empty<LibraryDto>();

            return libraries.Select(lib => new LibraryDto
            {
                Id = lib.LibraryId ?? Guid.Empty,
                Name = lib.Name ?? string.Empty,
                Address = lib.Address ?? string.Empty,
                City = lib.City,
                PostalCode = lib.Zip
            });
        }
        catch (Exception)
        {
            // Log error if needed
            return Enumerable.Empty<LibraryDto>();
        }
    }

    public async Task<LibraryDto?> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var library = await _apiClient.Api.Mobile.Libraries[id].GetAsync(cancellationToken: cancellationToken);

            if (library == null)
                return null;

            return new LibraryDto
            {
                Id = library.LibraryId ?? Guid.Empty,
                Name = library.Name ?? string.Empty,
                Address = library.Address ?? string.Empty,
                City = library.City,
                PostalCode = library.Zip
            };
        }
        catch (Exception)
        {
            // Log error if needed
            return null;
        }
    }
}
