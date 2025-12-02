namespace LaurelLibrary.Core.Models;

public class ReturnBookRequestDto
{
    public string Isbn { get; set; } = string.Empty;
    public int ReaderId { get; set; }
    public Guid LibraryId { get; set; }
}
