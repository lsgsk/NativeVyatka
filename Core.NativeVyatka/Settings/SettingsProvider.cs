using Plugin.Settings;
using Abstractions.Constants;
using Plugin.Settings.Abstractions;
using Abstractions.Interfaces.Settings;

namespace NativeVyatkaCore.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private ISettings Settings => CrossSettings.Current;

        public string Login
        {
            get => Settings.GetValueOrDefault(LoginKey, string.Empty);
            set => Settings.AddOrUpdateValue(LoginKey, value);
        }
        public string Password
        {
            get => Settings.GetValueOrDefault(PasswordKey, string.Empty);
            set => Settings.AddOrUpdateValue(PasswordKey, value);
        }

        public string UserHash
        {
            get => Settings.GetValueOrDefault(UserHashKey, string.Empty);
            set => Settings.AddOrUpdateValue(UserHashKey, value);
        }

        public string ServiceUrl
        {
            get => Settings.GetValueOrDefault(ServiceUrlKey, ApConstant.ServiceUrl);           
     
        }
        public string CsrfToken
        {
            get => Settings.GetValueOrDefault(CsrfTokenKey, string.Empty);
            set => Settings.AddOrUpdateValue(CsrfTokenKey, value);
        }
        public string SessionId
        {
            get => Settings.GetValueOrDefault(SessionIdKey, string.Empty);            
            set => Settings.AddOrUpdateValue(SessionIdKey, value);            
        }
        public string SessionName
        {
            get => Settings.GetValueOrDefault(SessionNameKey, string.Empty);          
            set => Settings.AddOrUpdateValue(SessionNameKey, value);            
        }

        public int LastSynchronization
        {
            get => Settings.GetValueOrDefault(LastSynchronizationKey, 0);
            set => Settings.AddOrUpdateValue(LastSynchronizationKey, value);
        }

        public void ClearPrefs()
        {
            foreach(var item in new[] { UserHashKey, ServiceUrlKey, CsrfTokenKey, SessionNameKey, SessionIdKey, LastSynchronizationKey })
            {
                Settings.Remove(item);
            }
            foreach (var item in new[] { LoginKey, PasswordKey })
            {
                Settings.Remove(item);
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
