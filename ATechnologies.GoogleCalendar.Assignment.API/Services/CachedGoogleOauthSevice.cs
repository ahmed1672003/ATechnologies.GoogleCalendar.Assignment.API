using ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;

using Microsoft.Extensions.Caching.Memory;

namespace ATechnologies.GoogleCalendar.Assignment.API.Services;

public class CachedGoogleOauthSevice : ICachedGoogleOauthSevice
{

    private readonly IMemoryCache _memoryCache;

    public CachedGoogleOauthSevice(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;

    }
    public async Task<string> SetAccessTokenAsync(GetGoogleAuthTokenDto Dto)
    {
        _memoryCache.Set("refresh_token", Dto.refresh_token, DateTimeOffset.Now.AddHours(5));
        return _memoryCache.Set("access_token", Dto.access_token, DateTimeOffset.Now.AddHours(1));
    }
    public async Task<string> GetAccessTokenAsync()
    {
        return _memoryCache.Get<string>("access_token");
    }

    public async Task<string> GetRefreshToken()
    {
        return _memoryCache.Get<string>("refresh_token");
    }
    public async Task UpdateAccessToken(string token)
    {
        _memoryCache.Set("access_token", token, DateTimeOffset.Now.AddHours(1));
    }
}
