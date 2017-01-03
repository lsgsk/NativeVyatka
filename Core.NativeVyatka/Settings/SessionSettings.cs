﻿using Abstractions.Interfaces.Settings;
using Plugin.Settings;
using Abstractions.Constants;
using System;
using Plugin.Settings.Abstractions;

namespace NativeVyatkaCore.Settings
{
    public class SessionSettings : ISessionSettings
    {
        private ISettings Settings => CrossSettings.Current;

        public string ServiceUrl
        {
            get
            {
                return Settings.GetValueOrDefault(ServiceUrlKey, ApConstant.ServiceUrl);
            }
            set
            {
                Settings.AddOrUpdateValue(ServiceUrlKey, value);
            }
        }
        public string CsrfToken
        {
            get
            {
               return Settings.GetValueOrDefault(CsrfTokenKey, string.Empty);
            }

            set
            {
                Settings.AddOrUpdateValue(CsrfTokenKey, value);
            }
        }
        public string PushToken
        {
            get
            {
                return Settings.GetValueOrDefault(PushTokenKey, string.Empty);
            }
            set
            {
                Settings.AddOrUpdateValue(PushTokenKey, value);
            }
        }
        public string SessionId
        {
            get
            {
                return Settings.GetValueOrDefault(SessionIdKey, string.Empty);
            }
            set
            {
                Settings.AddOrUpdateValue(SessionIdKey, value);
            }
        }
        public string SessionName
        {
            get
            {
                return Settings.GetValueOrDefault(SessionNameKey, string.Empty);
            }

            set
            {
                Settings.AddOrUpdateValue(SessionNameKey, value);
            }
        }

        public void ClearPrefs()
        {
            var array = new []{ ServiceUrlKey, PushTokenKey, CsrfTokenKey, SessionNameKey, SessionIdKey };
            foreach(var item in array)
            {
                Settings.Remove(item);
            }           
        }

        public const string ServiceUrlKey = "ServiceUrlKey";
        public const string PushTokenKey = "PushTokenKey";
        public const string CsrfTokenKey = "CsrfTokenKey";
        public const string SessionNameKey = "SessionNameKey";
        public const string SessionIdKey = "SessionIdKey";       
    }
}