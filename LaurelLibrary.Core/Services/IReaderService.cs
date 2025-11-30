using LaurelLibrary.Core.Models;

namespace LaurelLibrary.Core.Services;

public interface IReaderService
{
    Task<ReaderInfoDto?> GetReaderInfoAsync(int readerId, Guid libraryId, CancellationToken cancellationToken = default);
}
