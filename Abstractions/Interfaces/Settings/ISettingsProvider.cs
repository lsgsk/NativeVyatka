namespace Abstractions.Interfaces.Settings
{
    public interface ISettingsProvider
    {
        string Login { get; set; }
        string Password { get; set; }
        string UserHash { get; set; }
        string ServiceUrl { get; }
        string CsrfToken { get; set; }
        string SessionName { get; set; }
        string SessionId { get; set; }
        int LastSynchronization { get; set; }
        void ClearPrefs();
    }
}
