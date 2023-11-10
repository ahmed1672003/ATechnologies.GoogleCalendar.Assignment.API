namespace ATechnologies.GoogleCalendar.Assignment.API.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IGoogleOauthService _googleOauthService;
    private readonly ICachedGoogleOauthSevice _cachedGoogleOauthSevice;
    public UserController(IGoogleOauthService googleOauthService, ICachedGoogleOauthSevice cachedGoogleOauthSevice)
    {
        _googleOauthService = googleOauthService;
        _cachedGoogleOauthSevice = cachedGoogleOauthSevice;
    }

    /// <summary>
    /// get token by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost(Router.User.OauthUser)]
    public async Task<IActionResult> OauthUser([EmailAddress][Required] string email) => Ok(await _googleOauthService.GetOauthUri(email));

    /// <summary>
    /// redirect call back function from google
    /// </summary>
    /// <param name="code"></param>
    /// <param name="error"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    [HttpGet(Router.User.OauthCallBack)]
    public async Task<IActionResult> OauthCallBack(string? code, string? error, string? state)
    {
        try
        {
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            if (string.IsNullOrEmpty(code))
                return BadRequest();

            GetGoogleAuthTokenDto getGoogleAuthTokenDto = await _googleOauthService.GetTokenByCodeAsync(code);

            if (getGoogleAuthTokenDto is null)
                return Unauthorized();

            await SaveAuthInCookiesAsync(getGoogleAuthTokenDto);
            await _cachedGoogleOauthSevice.SetAccessTokenAsync(getGoogleAuthTokenDto);

            return Ok(getGoogleAuthTokenDto);
        }
        catch (GoogleApiException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// refresh token 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpPost(Router.User.RefreshToken)]
    public async Task<IActionResult> RefreshToken(string? refreshToken)
    {
        try
        {
            string refToken = refreshToken ?? HttpContext.Request.Cookies["refresh_token"]!;

            string cachedRefToken = await _cachedGoogleOauthSevice.GetRefreshToken();

            if (string.IsNullOrEmpty(refToken) && string.IsNullOrEmpty(cachedRefToken))
                return Unauthorized();

            GetRefreshTokenDto Dto = await _googleOauthService.RefreshTokenAsync(refToken ?? cachedRefToken);
            await UpdateAccessTokenInCookiesAsync(Dto);

            return Ok(Dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// revoke token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [HttpGet(Router.User.RevokeToken)]
    public async Task<IActionResult> RevokeToken(string? accessToken)
    {
        string token = accessToken ?? HttpContext.Request.Cookies["access_token"]!;
        string cahedToken = await _cachedGoogleOauthSevice.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(cahedToken))
            return Unauthorized();

        bool revokeTokenSuccess = await _googleOauthService.RevokeTokenAsync(token ?? cahedToken);
        if (!revokeTokenSuccess)
            return BadRequest("revoke token not success");
        else
        {
            await RemoveAuthInformationFromCookiesAsync();
            return Ok("revoke token success");
        }
    }

    private async Task SaveAuthInCookiesAsync(GetGoogleAuthTokenDto Dto)
    {
        HttpContext.Response.Cookies.Append("access_token", Dto.access_token, new CookieOptions()
        {
        });
        HttpContext.Response.Cookies.Append("refresh_token", Dto.refresh_token, new CookieOptions()
        {
        });
    }
    private async Task UpdateAccessTokenInCookiesAsync(GetRefreshTokenDto Dto)
    {
        HttpContext.Response.Cookies.Append("access_token", Dto.access_token, new CookieOptions()
        {
        });
        HttpContext.Response.Cookies.Append("id_token", Dto.id_token, new CookieOptions()
        {
        });
    }
    private async Task RemoveAuthInformationFromCookiesAsync()
    {
        HttpContext.Response.Cookies.Delete("access_token", new CookieOptions()
        {
        });
        HttpContext.Response.Cookies.Delete("refresh_token", new CookieOptions()
        {
        });
    }
}
