using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NativeVyatka
{
    public interface ISettingsProvider
    {
        Task<string> GetLoginAsync();
        Task SetLoginAsync(string value);
        Task<string> GetPasswordAsync();
        Task SetPasswordAsync(string value);
        string UserHash { get; set; }
        string ServiceUrl { get; }
        string CsrfToken { get; set; }
        string SessionName { get; set; }
        string SessionId { get; set; }
        int LastSynchronization { get; set; }
        void ClearPrefs();
    }

    public class SettingsProvider : ISettingsProvider
    {
        public async Task<string> GetLoginAsync() {
            try {
                return await SecureStorage.GetAsync(LoginKey);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                return string.Empty;
            }
        }

        public async Task SetLoginAsync(string value) {
            try {
                await SecureStorage.SetAsync(LoginKey, value);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
        }

        public async Task<string> GetPasswordAsync() {
            try {
                return await SecureStorage.GetAsync(PasswordKey);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                return string.Empty;
            }
        }

        public async Task SetPasswordAsync(string value) {
            try {
                await SecureStorage.SetAsync(PasswordKey, value);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
        }

        public string UserHash
        {
            get => Preferences.Get(UserHashKey, string.Empty);
            set => Preferences.Set(UserHashKey, value);
        }

        public string ServiceUrl
        {
            get => ApConstant.ServiceUrl;

        }
        public string CsrfToken
        {
            get => Preferences.Get(CsrfTokenKey, string.Empty);
            set => Preferences.Set(CsrfTokenKey, value);
        }
        public string SessionId
        {
            get => Preferences.Get(SessionIdKey, string.Empty);
            set => Preferences.Set(SessionIdKey, value);
        }
        public string SessionName
        {
            get => Preferences.Get(SessionNameKey, string.Empty);
            set => Preferences.Set(SessionNameKey, value);
        }

        public int LastSynchronization
        {
            get => Preferences.Get(LastSynchronizationKey, 0);
            set => Preferences.Set(LastSynchronizationKey, value);
        }

        public void ClearPrefs() {
            foreach (var item in new[] { UserHashKey, ServiceUrlKey, CsrfTokenKey, SessionNameKey, SessionIdKey, LastSynchronizationKey }) {
                Preferences.Remove(item);
            }
            foreach (var item in new[] { LoginKey, PasswordKey }) {
                SecureStorage.Remove(item);
            }
        }

        public const string LoginKey = "LoginKey";
        public const string PasswordKey = "PasswordKey";
        public const string UserHashKey = "LoginHashKey";
        public const string ServiceUrlKey = "ServiceUrlKey";
        public const string CsrfTokenKey = "CsrfTokenKey";
        public const string SessionNameKey = "SessionNameKey";
        public const string SessionIdKey = "SessionIdKey";
        public const string LastSynchronizationKey = "LastSynchronizationKet";
    }
}
