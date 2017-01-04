using Abstractions.Interfaces.Controllers;
using System;
using UIKit;
using Microsoft.Practices.Unity;
using Abstractions.Exceptions;

namespace NativeVyatkaIOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle)
        {
            mController = App.Container.Resolve<ILoginController>();            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mController.TryAutoLogin();

            var a = Plugin.Media.CrossMedia.Current;

#if DEBUG
            tfEmailView.Text = "RVbot";
            tfPasswordView.Text = "test";
#endif
        }

        async partial void OnSignInClick(UIButton sender)
        {
            try
            {
                tfEmailView.Layer.BorderColor = UIColor.Cyan.CGColor;
                await mController.Login(tfEmailView.Text, tfPasswordView.Text);
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                if (!string.IsNullOrEmpty(ex.EmailMessage))
                {
                    tfEmailView.Layer.BorderColor = UIColor.Red.CGColor; //ex.EmailMessage;
                    tfEmailView.BecomeFirstResponder();
                }
                if (!string.IsNullOrEmpty(ex.PasswordMessage))
                {
                    tfPasswordView.Layer.BorderColor = UIColor.Red.CGColor; //= ex.EmailMessage;
                    tfPasswordView.BecomeFirstResponder();
                }
            }
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }

        public ILoginController mController;
    }
}