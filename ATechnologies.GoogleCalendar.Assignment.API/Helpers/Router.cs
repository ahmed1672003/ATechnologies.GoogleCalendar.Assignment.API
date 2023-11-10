namespace ATechnologies.GoogleCalendar.Assignment.API.Helpers;
public static class Router
{
    private const string ApiPrefix = "api/";
    public static class User
    {
        private const string UserPrefix = ApiPrefix + "users/";
        public const string OauthUser = UserPrefix + "authenticate";
        public const string OauthCallBack = UserPrefix + "oauth-callback";
        public const string RefreshToken = UserPrefix + "refresh-token";
        public const string RevokeToken = UserPrefix + "revoke-token";
    }

    public static class GoogleCalendar
    {
        private const string GoogelCalendarPrefic = ApiPrefix + "calendars";
        public const string AddEvent = ApiPrefix + "add-event";
        public const string GetAllEvents = ApiPrefix + "get-all-events";
        public const string DeleteEventById = ApiPrefix + "delete-event-byid";
    }
}
