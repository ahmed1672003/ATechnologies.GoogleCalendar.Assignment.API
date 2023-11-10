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
    /// <summary>
    /// add new event
    /// </summary>
    /// <param name="Dto"></param>
    /// <param name="accessToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// get all events
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>

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
    /// <summary>
    /// Paginate events
    /// </summary>
    /// <remarks>
    ///    EventOrderBy
    ///    
    ///         [1] => StartDate
    ///         [2] => EndDate
    ///         [3] => Created
    ///         
    /// directon 
    /// 
    ///         [1] => ASC
    ///         [2] => DESC
    ///         
    /// </remarks>
    /// <param name="token"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="search"></param>
    /// <param name="eventOrderBy"></param>
    /// <param name="directon"></param>
    /// <returns></returns>
    [HttpGet(Router.GoogleCalendar.PaginateEvents)]
    public async Task<IActionResult> PaginateEvents(string token, int? pageNumber = 1, int? pageSize = 10, string? search = "", EventOrderBy? eventOrderBy = EventOrderBy.CreatedDate, OrderByDirection? directon = OrderByDirection.ASC)
    {
        try
        {
            string accessToken = token ?? HttpContext.Request.Cookies["access_token"] ?? await _cachedGoogleOauthSevice.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized();

            GetCalendarEventsDto Dto = JsonConvert.DeserializeObject<GetCalendarEventsDto>(await _calendarService.GetAllEventsAsnync(token));

            Func<GetEventDto, object> orderBy = eventDto => new();
            switch (eventOrderBy.Value)
            {
                case EventOrderBy.StartDate:
                    orderBy = eventDto => eventDto.Start.dateTime;
                    break;
                case EventOrderBy.EndDate:
                    orderBy = eventDto => eventDto.End.dateTime;
                    break;
                default:
                    orderBy = eventDto => eventDto.Created;
                    break;
            }
            List<GetEventDto> resultDtos = Dto.Items;

            if (directon != OrderByDirection.ASC)
                resultDtos.OrderByDescending(orderBy);

            resultDtos = resultDtos.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();

            resultDtos = resultDtos.Where(e =>
            e.Id.Contains(search) ||
            e.Status.Contains(search) ||
            e.Created.Contains(search) ||
            e.Updated.Contains(search) ||
            e.Start.dateTime.Contains(search) ||
            e.Start.timeZone.Contains(search) ||
            e.End.dateTime.Contains(search) ||
            e.End.timeZone.Contains(search) ||
            e.Kind.Contains(search) ||
            e.Location.Contains(search) ||
            e.Etag.Contains(search) ||
            e.Summary.Contains(search)).ToList();

            PaginationResponseResult<IEnumerable<GetEventDto>> result = new PaginationResponseResult<IEnumerable<GetEventDto>>()
            {
                CurrentPage = pageNumber.Value,
                PageSize = pageSize.Value,
                TotalCount = Dto.Items.Count,
                Items = resultDtos,
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// delete event by id
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
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
