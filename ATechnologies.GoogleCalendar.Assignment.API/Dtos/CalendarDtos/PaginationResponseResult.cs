namespace ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;

public sealed class PaginationResponseResult<TItem>
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public TItem Items { get; set; }
}
