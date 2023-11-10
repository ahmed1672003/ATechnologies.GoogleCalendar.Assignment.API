namespace ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;

public sealed class GetRefreshTokenDto
{
    public string access_token { get; set; }
    public long expires_in { get; set; }
    public string id_token { get; set; }
}
