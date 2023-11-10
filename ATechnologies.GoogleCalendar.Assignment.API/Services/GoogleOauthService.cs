using System.Text;

using ATechnologies.GoogleCalendar.Assignment.API.Dtos.AuthDtos;

using Google;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ATechnologies.GoogleCalendar.Assignment.API.Services;

public sealed class GoogleOauthService : IGoogleOauthService
{
    private readonly GoogleOauthSettings _googleOauthSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICachedGoogleOauthSevice _cachedGoogleOauthSevice;
    public GoogleOauthService(IOptions<GoogleOauthSettings> googleOauthOptions, IHttpClientFactory httpClientFactory, ICachedGoogleOauthSevice cachedGoogleOauthSevice)
    {
        _googleOauthSettings = googleOauthOptions.Value;
        _httpClientFactory = httpClientFactory;
        _cachedGoogleOauthSevice = cachedGoogleOauthSevice;
    }
    public async Task<string> GetOauthUri(string extraPram)
    {
        StringBuilder uri = new StringBuilder(_googleOauthSettings.OauthUri);

        var scope = _googleOauthSettings.Scopes.Aggregate((s1, s2) => string.Concat(s1, " ", s2));

        uri.Append($"client_id={_googleOauthSettings.ClientId}");
        uri.Append($"&redirect_uri={_googleOauthSettings.RedirectUri}");
        uri.Append($"&response_type={_googleOauthSettings.ResponseType}");
        uri.Append($"&scope={scope}");
        uri.Append($"&access_type={_googleOauthSettings.AccessType}");
        uri.Append($"&state={extraPram}");
        uri.Append($"&approval_prompt={_googleOauthSettings.ApprovalPrompt}");
        return uri.ToString();
    }
    public async Task<GetGoogleAuthTokenDto> GetTokenByCodeAsync(string code)
    {
        GetGoogleAuthTokenDto dto = null;
        var postData = new
        {
            code = code,
            client_id = _googleOauthSettings.ClientId,
            client_secret = _googleOauthSettings.ClientSecret,
            redirect_uri = _googleOauthSettings.RedirectUri,
            grant_type = "authorization_code"
        };
        using (HttpClient httpClient = _httpClientFactory.CreateClient())
        {
            StringContent content = new(JsonConvert.SerializeObject(postData), Encoding.ASCII, "application/json");
            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(_googleOauthSettings.TokenUri, content))
                {
                    if (!response.IsSuccessStatusCode)
                        return dto;
                    string responseStrign = await response.Content.ReadAsStringAsync();
                    dto = JsonConvert.DeserializeObject<GetGoogleAuthTokenDto>(responseStrign)!;
                    await _cachedGoogleOauthSevice.SetAccessTokenAsync(dto);

                    return dto;
                }
            }
            catch (Exception ex)
            {
                throw new GoogleApiException("GoogleOauthServices.GetTokenByCodeAsync");
            }
        }
    }
    private async Task<string> GetRefreshTokenUriAsync(string refreshToken)
    {
        StringBuilder uri = new StringBuilder(_googleOauthSettings.RefreshTokenUri);
        uri.Append($"client_id={_googleOauthSettings.ClientId}");
        uri.Append($"client_secret={_googleOauthSettings.ClientSecret}");
        uri.Append($"refresh_token={refreshToken}");

        return uri.ToString();
    }
    public async Task<GetRefreshTokenDto> RefreshTokenAsync(string refreshToken)
    {
        var postData = new
        {
            client_id = _googleOauthSettings.ClientId,
            client_secret = _googleOauthSettings.ClientSecret,
            grant_type = "refresh_token",
            refresh_token = refreshToken
        };
        using (HttpClient httpClient = _httpClientFactory.CreateClient())
        {
            StringContent content = new(JsonConvert.SerializeObject(postData), Encoding.ASCII, "application/json");
            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(_googleOauthSettings.RefreshTokenUri, content))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new GoogleApiException("GoogleOauthServices.RefreshTokenAsync");

                    string responseStrign = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<GetRefreshTokenDto>(responseStrign)!;

                    await _cachedGoogleOauthSevice.UpdateAccessToken(data.access_token);
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new GoogleApiException("GoogleOauthServices.RefreshTokenAsync");
            }
        }
    }

    public async Task<bool> RevokeTokenAsync(string accessToken)
    {
        var postData = new
        {
            token = accessToken,
        };
        try
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.ASCII, "application/json");
                using (HttpResponseMessage response = await httpClient.PostAsync(_googleOauthSettings.RevokeTokenUri, content))
                {
                    if (!response.IsSuccessStatusCode)
                        return false;

                    string responseStrign = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(responseStrign)!;

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            throw new GoogleApiException("GoogleOauthServices.RefreshTokenAsync");
        }
    }
}
