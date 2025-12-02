namespace LaurelLibrary.Core.Models;

public class BorrowedBookDto
{
    public Guid BookId { get; set; }
    public int BookInstanceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Isbn { get; set; }
    public string? BookUrl { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? BorrowedDate { get; set; }

    // Helper properties for UI
    public int DaysRemaining
    {
        get
        {
            if (DueDate == null)
                return 0;

            var days = (int)(DueDate.Value.Date - DateTime.Today).TotalDays;
            return days;
        }
    }

    public bool IsOverdue => DaysRemaining < 0;

    public bool IsDueSoon => !IsOverdue && DaysRemaining >= 0 && DaysRemaining <= 7;

    public string DueStatus
    {
        get
        {
            if (DueDate == null)
                return "No due date";

            if (IsOverdue)
                return $"Overdue by {Math.Abs(DaysRemaining)} day{(Math.Abs(DaysRemaining) != 1 ? "s" : "")}";

            if (IsDueSoon)
                return $"Due in {DaysRemaining} day{(DaysRemaining != 1 ? "s" : "")}";

            return $"Due in {DaysRemaining} day{(DaysRemaining != 1 ? "s" : "")}";
        }
    }

    public string StatusColor
    {
        get
        {
            if (DueDate == null)
                return "Tertiary"; // Neutral color

            return IsOverdue ? "Danger" : IsDueSoon ? "Warning" : "Success";
        }
    }
}
