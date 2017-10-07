using System;
using System.Collections.Generic;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Plugin.CurrentActivity;
using Android.Content;

namespace NativeVyatkaAndroid.Utilities
{
    public class PageNavigator : ICrossPageNavigator
    {
        public PageNavigator(IPageTypeImplementation implementation)
        {
            this.mImplementation = implementation;
        }

        public void GoToPage(PageStates state, Dictionary<string, string> extras = null, bool closePrevious = false)
        {
            var activity = CrossCurrentActivity.Current.Activity;
            if (activity != null)
            {
                var intent = GetIntentFor(state);
                if (extras != null)
                {
                    foreach (var item in extras)
                    {
                        intent.PutExtra(item.Key, item.Value);
                    }
                }
                activity.StartActivity(intent);   
                if(closePrevious)
                {
                    activity.Finish();
                }
            }
        }

        private Intent GetIntentFor(PageStates state)
        {
            if (mImplementation == null)
            {
                throw new InvalidOperationException("No IFormTypeImplementation");
            }
            var intent = new Intent(CrossCurrentActivity.Current.Activity.BaseContext, mImplementation.GetTypeFor(state));
            return intent;
        }

        public void Goback()
        {
            CrossCurrentActivity.Current.Activity.Finish();
        }

        private readonly IPageTypeImplementation mImplementation;
    }
}