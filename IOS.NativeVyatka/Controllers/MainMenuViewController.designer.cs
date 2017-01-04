// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace NativeVyatkaIOS.Controllers
{
    [Register ("MainMenuViewController")]
    partial class MainMenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgProfilePhoto { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbProfileEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbProfileName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tvMenuList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgProfilePhoto != null) {
                imgProfilePhoto.Dispose ();
                imgProfilePhoto = null;
            }

            if (lbProfileEmail != null) {
                lbProfileEmail.Dispose ();
                lbProfileEmail = null;
            }

            if (lbProfileName != null) {
                lbProfileName.Dispose ();
                lbProfileName = null;
            }

            if (tvMenuList != null) {
                tvMenuList.Dispose ();
                tvMenuList = null;
            }
        }
    }
}