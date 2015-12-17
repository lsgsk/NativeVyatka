
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
using Android.Accounts;

namespace NativeVyatkaAndroid
{   
    public class AppAccount : Account 
    {
        public const string TYPE = "vyatka.lsgsk.account";

        public AppAccount(Parcel parcel)  : base(parcel)
        {            
        }

        public AppAccount(string name) : base(name, TYPE)
        {
        }
    }
}

