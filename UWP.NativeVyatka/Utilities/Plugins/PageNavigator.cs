using Abstractions.Interfaces.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NativeVyatka.UWP.Utilities
{
    public class PageNavigator: ICrossPageNavigator
    {
        public PageNavigator(IPageTypeImplementation implementation)
        {
            this.mImplementation = implementation;
        }
        private readonly IPageTypeImplementation mImplementation;

        public void GoToPage(PageStates state, Dictionary<string, string> extras = null)
        {
            var frame = (Window.Current?.Content as Frame);
            if (frame != null)
            {
                frame.Navigate(GetTypeFor(state), extras);
                frame.BackStack.Remove(frame.BackStack.LastOrDefault());
            }
        }

        private Type GetTypeFor(PageStates state)
        {
            return mImplementation.GetTypeFor(state);
        }
    }
}
