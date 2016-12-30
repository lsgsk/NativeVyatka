// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace NativeVyatkaIOS
{
    [Register ("UILoginController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfEmailView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfPasswordView { get; set; }

        [Action ("OnSignInClick:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnSignInClick (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (tfEmailView != null) {
                tfEmailView.Dispose ();
                tfEmailView = null;
            }

            if (tfPasswordView != null) {
                tfPasswordView.Dispose ();
                tfPasswordView = null;
            }
        }
    }
}