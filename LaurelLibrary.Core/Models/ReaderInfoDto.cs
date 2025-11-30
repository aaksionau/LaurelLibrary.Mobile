namespace LaurelLibrary.Core.Models;

public class ReaderInfoDto
{
    public int ReaderId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public IEnumerable<BorrowedBookDto> BorrowedBooks { get; set; } = Enumerable.Empty<BorrowedBookDto>();
}
