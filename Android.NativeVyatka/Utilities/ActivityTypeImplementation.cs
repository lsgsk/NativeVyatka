using System;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;

namespace NativeVyatkaAndroid.Utilities
{
    public class ActivityTypeImplementation : IPageTypeImplementation
    {
        public Type GetTypeFor(PageStates state)
        {
            Type type = null;
            switch (state)
            {
                case PageStates.LoginPage:
                    type = typeof(LoginActivity);
                    break;
                case PageStates.BulialListPage:
                    type = typeof(MainActivity);
                    break;
                case PageStates.BurialEditPage:
                    type = typeof(BurialEditActivity);
                    break;
            }
            return type;
        }
    }
}