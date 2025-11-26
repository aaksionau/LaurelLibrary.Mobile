using LaurelLibrary.Core.Models;

namespace LaurelLibrary.Core.Services;

public interface ILibraryService
{
    Task<IEnumerable<LibraryDto>> SearchLibrariesAsync(string query, CancellationToken cancellationToken = default);
    Task<LibraryDto?> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
