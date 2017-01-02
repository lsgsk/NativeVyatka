namespace Abstractions.Interfaces.Settings
{
    public interface ISessionSettings
    {
        string ServiceUrl { get; set; }
        string PushToken { get; set; }
        string CsrfToken { get; set; }
        string SessionName { get; set; }
        string SessionId { get; set; }
    }
}
