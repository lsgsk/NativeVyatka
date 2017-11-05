namespace Abstractions.Interfaces.Settings
{
    public interface ISettingsProvider
    {
        string ServiceUrl { get; set; }
        string PushToken { get; set; }
        string CsrfToken { get; set; }
        string SessionName { get; set; }
        string SessionId { get; set; }
        int LastSynchronization { get; set; }
        void ClearPrefs();
    }
}
