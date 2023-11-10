namespace ATechnologies.GoogleCalendar.Assignment.API.Services;
public sealed class GoogleCalendarService : IGoogleCalendarService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GooglCalendarSettings _googlCalendarSettings;
    private readonly GoogleOauthSettings _googleOauthSettings;
    public GoogleCalendarService(
        IHttpClientFactory httpClientFactory,
        IOptions<GooglCalendarSettings> calendarOptions,
        IOptions<GoogleOauthSettings> OauthOptions)
    {
        _httpClientFactory = httpClientFactory;
        _googlCalendarSettings = calendarOptions.Value;
        _googleOauthSettings = OauthOptions.Value;
    }
    public async Task<GetEventDto> AddEventAsync(AddEventDto Dto, string token)
    {
        Dto.start.dateTime = DateTime.Parse(Dto.start.dateTime).ToString("yyy-MM-dd'T'HH:mm:ss.fff");
        Dto.end.dateTime = DateTime.Parse(Dto.end.dateTime).ToString("yyy-MM-dd'T'HH:mm:ss.fff");
        try
        {
            string model = JsonConvert.SerializeObject(Dto, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            RestClient restClient = new RestClient(new Uri(_googlCalendarSettings.AddEventUri));
            RestRequest request = new RestRequest();
            request.AddQueryParameter("key", _googlCalendarSettings.ApiKey);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", model, ParameterType.RequestBody);
            RestResponse response = await restClient.PostAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;


            GetEventDto createdDto = JsonConvert.DeserializeObject<GetEventDto>(response.Content);

            return createdDto;
        }
        catch (Exception ex)
        {
            throw new GoogleApiException("GoogleCalendarService.AddEventAsync", ex.Message);
        }
    }
    public async Task<string> GetAllEventsAsnync(string token)
    {
        RestClient restClient = new RestClient(new Uri(_googlCalendarSettings.AllEvenetsUri));
        RestRequest request = new RestRequest();
        request.AddQueryParameter("key", _googlCalendarSettings.ApiKey);
        request.AddHeader("Authorization", "Bearer " + token);
        request.AddHeader("Accept", "application/json");

        try
        {
            var response = await restClient.GetAsync(request);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            return response.Content;
        }
        catch (Exception ex)
        {
            throw new GoogleApiException($"{nameof(GoogleCalendarService)}.{nameof(GetAllEventsAsnync)}", ex.Message);
        }
    }
    public async Task<bool> DeleteEventById(string id, string token)
    {
        RestClient restClient = new RestClient(new Uri($"{_googlCalendarSettings.DeleteEventUri}/{id}?key={_googlCalendarSettings.ApiKey}"));
        RestRequest request = new RestRequest();
        request.AddHeader("Authorization", "Bearer " + token);
        request.AddHeader("Accept", "application/json");
        try
        {
            RestResponse response = await restClient.DeleteAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            throw new GoogleApiException("GoogleCalendarService.AddEventAsync", ex.Message);
        }
    }
}
