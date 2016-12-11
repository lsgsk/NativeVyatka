using System;
using Abstractions.Interfaces.Settings;
using Plugin.Settings;
using Abstractions.Constants;

namespace NativeVyatkaCore.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        public string ServiceUrl
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(ServiceUrlKey, Common.ServiceUrl);
            }

            set
            {
                CrossSettings.Current.AddOrUpdateValue(ServiceUrlKey, value);
            }
        }
        public string CsrfToken
        {
            get
            {
               return CrossSettings.Current.GetValueOrDefault(CsrfTokenKey, string.Empty);
            }

            set
            {
                CrossSettings.Current.AddOrUpdateValue(CsrfTokenKey, value);
            }
        }
        public string PushToken
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(PushTokenKey, string.Empty);
            }

            set
            {
                CrossSettings.Current.AddOrUpdateValue(PushTokenKey, value);
            }
        }
        public string SessionId
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(SessionIdKey, string.Empty);
            }

            set
            {
                CrossSettings.Current.AddOrUpdateValue(SessionIdKey, value);
            }
        }
        public string SessionName
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(SessionNameKey, string.Empty);
            }

            set
            {
                CrossSettings.Current.AddOrUpdateValue(SessionNameKey, value);
            }
        }
        public const string ServiceUrlKey = "ServiceUrlKey";
        public const string PushTokenKey = "PushTokenKey";
        public const string CsrfTokenKey = "CsrfTokenKey";
        public const string SessionNameKey = "SessionNameKey";
        public const string SessionIdKey = "SessionIdKey";
    }
}
