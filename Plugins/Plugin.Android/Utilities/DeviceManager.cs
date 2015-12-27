using System;
using Android.Content;
using Android.Locations;
using Android.Telephony;
using Android.Content.PM;
using Android.Util;
using Android.Net.Wifi;
using Android.OS;
using Android.Accounts;

namespace Plugins
{
    public sealed class DeviceManager
    {
        public DeviceManager(Context context)
        {
            //Ettention. Manifest does not include all pemissions, which need for this manager
            this.mContext = context;
        }

        public String GetImei()
        {
            var telephonyManager = (TelephonyManager)mContext.GetSystemService(Context.TelephonyService);
            return telephonyManager.DeviceId;
        }

        public Location GetLastKnownCoordinates()
        {
            var mLocManager = (LocationManager)mContext.GetSystemService(Context.LocationService);
            var providers = mLocManager.AllProviders;
            Location location;
            Location best = null;
            for (int i = providers.Count - 1; i >= 0; i--)
            {
                location = mLocManager.GetLastKnownLocation(providers[i]);
                if (location == null)
                    continue;
                if (best == null || best.Accuracy > location.Accuracy)
                    best = location;
            }
            return best; //а шо нужно, если нул
        }

        public int GetVersionCode()
        {
            try
            {
                var pInfo = mContext.PackageManager.GetPackageInfo(GetPackageName(), 0);
                return pInfo.VersionCode;
            }
            catch (PackageManager.NameNotFoundException ex)
            {
                iConsole.Error(ex.ToString());
            }
            return -1;
        }

        public string GetVersion()
        {
            try
            {
                return mContext.PackageManager.GetPackageInfo(GetPackageName(), 0).VersionName;
            }
            catch (Exception ex)
            {
                iConsole.Error(ex.ToString());
                return String.Empty;
            }
        }

        public String GetMacAddress()
        {
            var manager = (WifiManager)mContext.GetSystemService(Context.WifiService);
            var info = manager.ConnectionInfo;
            return info.MacAddress;
        }

        public String GetPackageName()
        {
            return mContext.ApplicationContext.PackageName;
        }

        /*public String getPhoneNumber()
    {
        TelephonyManager tMgr = (TelephonyManager) getSystemService(Context.TELEPHONY_SERVICE);
        String mPhoneNumber = tMgr.getLine1Number();
        return mPhoneNumber;
    }*/

        public string GetOsVersion()
        {
            return Build.VERSION.Release;
        }

        public String GetDeviceInfo()
        {
            return GetDeviceName() + "_" + GetOsVersion();
        }

        public String GetDeviceName()
        {
            String manufacturer = Build.Manufacturer;
            String model = Build.Model;
            if (model.StartsWith(manufacturer))
            {
                return capitalize(model).Replace(" ", "_");
            }
            else
            {
                return capitalize(manufacturer) + "_" + model;
            }
        }

        private static String capitalize(String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }
            char first = s[0];
            if (Char.IsUpper(first))
            {
                return s;
            }
            else
            {
                return Char.ToUpper(first) + s.Substring(1);
            }
        }

        public String GetUserEmail()
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.EclairMr1) // > 7)
            {
                var emailPattern = Patterns.EmailAddress; // API level 8+
                var accounts = AccountManager.Get(mContext).GetAccountsByType("com.google");// getAccounts();
                if (accounts != null)
                {
                    foreach (var account in accounts)
                    {
                        if (emailPattern.Matcher(account.Name).Matches())
                        {
                            return account.Name; //только первый email
                        }
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public String GetLanguage()
        {
            return Java.Util.Locale.Default.Language;
        }

        private readonly Context mContext;
    }
}

