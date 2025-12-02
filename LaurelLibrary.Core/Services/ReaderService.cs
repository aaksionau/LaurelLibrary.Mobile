using LaurelLibrary.Core.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace LaurelLibrary.Core.Services;

public class ReaderService : IReaderService
{
    private readonly LaurelLibrary.ApiClient.ApiClient _apiClient;
    private readonly string _blobStorageDomain;

    public ReaderService(HttpClient httpClient, string baseUrl = "http://localhost:5083", string blobStorageDomain = "https://mylibrarianprodza8twn.blob.core.windows.net")
    {
        var adapter = new HttpClientRequestAdapter(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient
        );
        adapter.BaseUrl = baseUrl;
        _apiClient = new ApiClient.ApiClient(adapter);
        _blobStorageDomain = blobStorageDomain;
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
                LibraryName = library.Name,
                BorrowedBooks = response?.CurrentBorrowedBooks?.Select(book => new BorrowedBookDto
                {
                    BookInstanceId = book.BookInstanceId ?? 0,
                    Title = book.BookTitle ?? string.Empty,
                    Author = book.AuthorNames,
                    BookUrl = !string.IsNullOrEmpty(book.BookUrl) ? $"{_blobStorageDomain}/{book.BookUrl}" : null,
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

    public async Task<bool> ReturnBookRequestAsync(int readerId, Guid libraryId, int bookInstanceId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Call the return/request endpoint: /api/mobile/return/request
            // The endpoint requires BookInstanceIds, ReaderId, and LibraryId
            var response = await _apiClient.Api.Mobile.Return.Request.PostAsync(
                new ApiClient.Models.MobileReturnRequestDto() 
                { 
                    ReaderId = readerId, 
                    LibraryId = libraryId,
                    BookInstanceIds = new List<int?> { bookInstanceId }
                }, 
                cancellationToken: cancellationToken);

            return response != null;
        }
        catch (Exception)
        {
            // Log error if needed
            return false;
        }
    }
}
