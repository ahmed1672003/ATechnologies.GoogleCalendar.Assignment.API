namespace ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;

public sealed class GetEventDto
{
    public string Id { get; set; }
    public string Kind { get; set; }
    public string Etag { get; set; }
    public string Status { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public string Summary { get; set; }
    public EventDateTiemDto Start { get; set; }
    public EventDateTiemDto End { get; set; }
}

public sealed class GetCalendarEventsDto
{
    public IEnumerable<GetEventDto> Items { get; set; }
}