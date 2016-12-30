using Abstractions.Interfaces.Plugins;
using System.Collections.Generic;
using Abstractions.Models;

namespace NativeVyatkaIOS.Utilities
{
    public class PageNavigator : ICrossPageNavigator
    {
        public void GoToPage(PageStates state, Dictionary<string, string> extras = null)
        {            
        }
    }
}