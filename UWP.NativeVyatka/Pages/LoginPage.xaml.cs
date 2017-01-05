using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Controllers;
using Abstractions.Exceptions;

namespace NativeVyatka.UWP.Pages
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            mController = App.Container.Resolve<ILoginController>();           
        }        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //mController.TryAutoLogin();
#if DEBUG
            tbEmail.Text = "RVbot";
            tbPassword.Text = "test";
#endif
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            mController.Dispose();
        }

        private async void OnSignInClick(object sender, RoutedEventArgs e)
        {
            try
            {
                tbEmailError.Text = tbPasswordError.Text = string.Empty;
                await mController.Login(tbEmail.Text, tbPassword.Text);
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                if (!string.IsNullOrEmpty(ex.EmailMessage))
                {
                    tbEmailError.Text = ex.EmailMessage;
                    tbEmail.Focus(FocusState.Programmatic);
                }
                if (!string.IsNullOrEmpty(ex.PasswordMessage))
                {
                    tbPasswordError.Text = ex.EmailMessage;
                    tbPassword.Focus(FocusState.Programmatic);
                }
            }
        }
    public ILoginController mController;
}
}
