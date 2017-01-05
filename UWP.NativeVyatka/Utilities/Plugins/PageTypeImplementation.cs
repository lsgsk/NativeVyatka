using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using NativeVyatka.UWP.Pages;
using System;

namespace NativeVyatka.UWP.Utilities
{
    public class PageTypeImplementation : IPageTypeImplementation
    {
        public Type GetTypeFor(PageStates state)
        {
            Type type = null;
            switch (state)
            {
                case PageStates.LoginPage:
                    type = typeof(LoginPage);
                    break;
                case PageStates.BulialListPage:
                    type = typeof(MainPage);
                    break;
                case PageStates.BurialEditPage:
                    type = typeof(BurialEditPage);
                    break;
            }
            return type;
        }
    }
}
