using LaurelLibrary.Core.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace LaurelLibrary.Core.Services;

public class ReaderService : IReaderService
{
    private readonly LaurelLibrary.ApiClient.ApiClient _apiClient;

    public ReaderService(HttpClient httpClient, string baseUrl = "http://localhost:5083")
    {
        var adapter = new HttpClientRequestAdapter(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient
        );
        adapter.BaseUrl = baseUrl;
        _apiClient = new ApiClient.ApiClient(adapter);
    }

    public async Task<ReaderInfoDto?> GetReaderInfoAsync(int readerId, Guid libraryId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Call the reader info endpoint: /api/mobile/reader/{readerId}/info
            var response = await _apiClient.Api.Mobile.Reader[readerId.ToString()].Info.GetAsync((opt) => opt.QueryParameters.LibraryId = libraryId, cancellationToken);

            if (response == null)
                return null;

            var reader = response?.Reader;

            var library = response?.Libraries?.FirstOrDefault(x => x.LibraryId == libraryId);

            if (reader == null || library == null) 
                return null;

            return new ReaderInfoDto
            {
                ReaderId = readerId,
                Name = reader.FirstName,
                Email = reader.Email,
                BorrowedBooks = response?.CurrentBorrowedBooks?.Select(book => new BorrowedBookDto
                {
                    Title = book.BookTitle ?? string.Empty,
                    Author = book.AuthorNames,
                    Isbn = book.BookIsbn,
                    DueDate = book.DueDate.HasValue ? book.DueDate.Value.Date : null,
                    BorrowedDate = book.CheckedOutDate.HasValue ? book.CheckedOutDate.Value.Date : null,
                }) ?? Enumerable.Empty<BorrowedBookDto>()
            };
        }
        catch (Exception)
        {
            // Log error if needed
            return null;
        }
    }
}
