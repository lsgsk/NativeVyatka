using System;
using Android.Content;
using Android.OS;
using Android.Support.V7.Preferences;
using Plugins;

namespace NativeVyatkaAndroid
{
    public class SettingsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public static SettingsFragment NewInstance()
        {
            var frag = new SettingsFragment();
            return frag;
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.settings);
            var prefIds = new []{ Resource.String.key_user_name, Resource.String.key_photo_tag, Resource.String.key_camera };
            foreach(var item in prefIds)
            {
                var editTextPreference = FindPreference(GetString(item)) as EditTextPreference;
                if (editTextPreference != null)
                {
                    editTextPreference.Summary = editTextPreference.Text;
                }
            }
        }

        public override void OnResume()
        {
           base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(Activity).RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();
            PreferenceManager.GetDefaultSharedPreferences(Activity).UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, String key)
        {
            try
            {
                Preference pref = FindPreference(key);
                var editTextPreference = pref as EditTextPreference;
                if (editTextPreference != null)
                {
                    editTextPreference.Summary = editTextPreference.Text; 
                }
            } catch (Exception ex)
            {
                iConsole.Error(ex);
            }
        }
    }
}

