using Plugin.Settings;
using Abstractions.Constants;
using Plugin.Settings.Abstractions;
using Abstractions.Interfaces.Settings;

namespace NativeVyatkaCore.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private ISettings Settings => CrossSettings.Current;

        public string LoginHash
        {
            get => Settings.GetValueOrDefault(LoginHashKey, string.Empty);
            set => Settings.AddOrUpdateValue(LoginHashKey, value);
        }

        public string ServiceUrl
        {
            get => Settings.GetValueOrDefault(ServiceUrlKey, ApConstant.ServiceUrl);            
            set => Settings.AddOrUpdateValue(ServiceUrlKey, value);            
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
            var array = new []{ ServiceUrlKey, CsrfTokenKey, SessionNameKey, SessionIdKey, LastSynchronizationKey };
            foreach(var item in array)
            {
                Settings.Remove(item);
            }           
        }

        public const string LoginHashKey = "LoginHashKey";
        public const string ServiceUrlKey = "ServiceUrlKey";
        public const string CsrfTokenKey = "CsrfTokenKey";
        public const string SessionNameKey = "SessionNameKey";
        public const string SessionIdKey = "SessionIdKey";
        public const string LastSynchronizationKey = "LastSynchronizationKet";
    }
}
