using Abstractions.Interfaces.Plugins;
using System;
using Abstractions.Models;

namespace NativeVyatkaIOS.Utilities
{
    public class ControllersTypeImplementation : IPageNameImplementation
    {
        public string GetNameFor(PageStates state)
        {            
            switch (state)
            {
                case PageStates.LoginPage:
                    return "LoginViewController";
                case PageStates.BulialListPage:
                    return "MainViewController";
                case PageStates.BurialEditPage:
                    break;
            }
            throw new NotSupportedException();
        }
    }
}
