namespace ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;

public sealed class EventDateTiemDto
{
    [DataType(DataType.DateTime)]
    public string dateTime { get; set; }
    public string timeZone { get; set; }

    public EventDateTiemDto()
    {
        timeZone = "UTC";
    }
}