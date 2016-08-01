
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "SettingsActivity", Theme = "@style/AppTheme")]            
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_SettingsActivity);
            var frag = SettingsFragment.NewInstance();
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_container, frag).Commit();
        }
    }
}

