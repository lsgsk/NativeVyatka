using Abstractions.Models;
using System;
using System.Collections.Generic;

namespace Abstractions.Interfaces.Plugins
{
    public interface ICrossPageNavigator
    {
        void GoToPage(PageStates state, Dictionary<string, string> extras = null);
    }

    public interface IPageTypeImplementation
    {
        Type GetTypeFor(PageStates state);
    }

    public interface IPageNameImplementation
    {
        string GetNameFor(PageStates state);
    }

    public static class FormBundleConstants
    { 
        public const string BurialModel = "BurialModel";
    }
}
