using ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;
namespace ATechnologies.GoogleCalendar.Assignment.API.IServices;

public interface IGoogleOauthService
{
    Task<string> GetOauthUri(string extraPram);
    Task<GetGoogleAuthTokenDto> GetTokenByCodeAsync(string code);
    Task<GetRefreshTokenDto> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string accessToken);
}
