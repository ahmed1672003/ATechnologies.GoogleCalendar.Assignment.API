namespace ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;

public sealed class AddEventDto
{
    public string summary { get; set; }
    public string description { get; set; }

    public EventDateTiemDto start { get; set; }
    public EventDateTiemDto end { get; set; }

}
