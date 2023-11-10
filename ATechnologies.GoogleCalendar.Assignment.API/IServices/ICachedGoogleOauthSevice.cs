using ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;

namespace ATechnologies.GoogleCalendar.Assignment.API.IServices;

public interface ICachedGoogleOauthSevice
{
    Task<string> SetAccessTokenAsync(GetGoogleAuthTokenDto Dto);
    Task<string> GetAccessTokenAsync();
    Task<string> GetRefreshToken();
    Task UpdateAccessToken(string token);
}
