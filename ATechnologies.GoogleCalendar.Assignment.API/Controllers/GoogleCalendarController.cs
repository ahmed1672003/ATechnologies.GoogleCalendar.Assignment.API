using System.Net;

using ATechnologies.GoogleCalendar.Assignment.API.Dtos.CalendarDtos;

using Newtonsoft.Json;

namespace ATechnologies.GoogleCalendar.Assignment.API.Controllers;

[ApiController]
public class GoogleCalendarController : ControllerBase
{
    private readonly IGoogleCalendarService _calendarService;
    private readonly ICachedGoogleOauthSevice _cachedGoogleOauthSevice;
    public GoogleCalendarController(IGoogleCalendarService calendarService, ICachedGoogleOauthSevice cachedGoogleOauthSevice)
    {
        _calendarService = calendarService;
        _cachedGoogleOauthSevice = cachedGoogleOauthSevice;
    }

    [HttpPost(Router.GoogleCalendar.AddEvent)]
    public async Task<IActionResult> AddEventAsync([FromForm] AddEventDto Dto, string? accessToken)
    {
        string token = accessToken ?? HttpContext.Request.Cookies["access_token"] ?? await _cachedGoogleOauthSevice.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
            return Unauthorized();
        try
        {
            var createdDto = await _calendarService.AddEventAsync(Dto, token);
            if (createdDto is null)
                return BadRequest("add event not success !");

            return Created("", createdDto);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet(Router.GoogleCalendar.GetAllEvents)]
    public async Task<IActionResult> GetAllEventsAsync(string? token)
    {
        try
        {
            string accessToken = token ?? HttpContext.Request.Cookies["access_token"] ?? await _cachedGoogleOauthSevice.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized();
            string json = await _calendarService.GetAllEventsAsnync(token);
            if (json is null)
                return BadRequest();

            GetCalendarEventsDto result = JsonConvert.DeserializeObject<GetCalendarEventsDto>(json);

            return Ok(result);
        }
        catch (Exception ex)
        {

            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

        }
    }

    [HttpDelete(Router.GoogleCalendar.DeleteEventById)]
    public async Task<IActionResult> DeleteEventById(string eventId, string? token)
    {
        try
        {
            string accessToken = token ?? HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized();

            bool success = await _calendarService.DeleteEventById(eventId, accessToken);

            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
