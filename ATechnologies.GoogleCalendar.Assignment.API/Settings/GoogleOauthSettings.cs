namespace ATechnologies.GoogleCalendar.Assignment.API.Settings;
public class GoogleOauthSettings
{
    public string ApplicationName { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
    public string OauthUri { get; set; }
    public List<string> Scopes { get; set; }
    public string ProjectId { get; set; }
    public string TokenUri { get; set; }
    public string AuthProviderX509CertUrl { get; set; }
    public string ResponseType { get; set; }
    public string AccessType { get; set; }
    public string ApprovalPrompt { get; set; }
    public string RefreshTokenUri { get; set; }
    public string RevokeTokenUri { get; set; }
}
