
namespace ATechnologies.GoogleCalendar.Assignment.API.IServices;

public interface IGoogleCalendarService
{
    //Task<Event> AddEventAsync(AddEventDto Dto, string token);
    Task<GetEventDto> AddEventAsync(AddEventDto Dto, string token);
    Task<string> GetAllEventsAsnync(string token);
    Task<bool> DeleteEventById(string id, string token);
}
