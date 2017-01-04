using Abstractions.Interfaces.Plugins;
using System.Collections.Generic;
using Abstractions.Models;
using UIKit;

namespace NativeVyatkaIOS.Utilities
{
    public class PageNavigator : ICrossPageNavigator
    {
        public PageNavigator(IPageNameImplementation implementation)//, UIStoryboard storyboard, UINavigationController navigation)
        {
            this.mImplementation = implementation;
            //this.mStoryboard = storyboard;
            //this.mNavigation = navigation;
        }   

        public void GoToPage(PageStates state, Dictionary<string, string> extras = null)
        {
            var controller = mStoryboard.InstantiateViewController(mImplementation.GetNameFor(state));
            mNavigation.PushViewController(controller, true);
        }

        private readonly IPageNameImplementation mImplementation;
        private readonly UIStoryboard mStoryboard;
        private readonly UINavigationController mNavigation;
    }
}