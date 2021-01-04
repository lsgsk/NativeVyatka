using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.App;
using Plugin.Permissions;
using Unity;

namespace NativeVyatka.Common.Activity
{
    [Activity(Theme = "@style/AppTheme", Icon = "@mipmap/ic_launcher", WindowSoftInputMode = SoftInput.StateAlwaysHidden, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class ApplicationActivity : AppCompatActivity
    {
        private readonly IRouter router = App.Container.Resolve<IRouter>();

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ApplicationActivity);
            if (savedInstanceState == null) {
                this.router.OpenLoginScreen();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults) {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed() {
            var fragmentList = SupportFragmentManager.Fragments;
            bool handled = false;
            foreach (var f in fragmentList) {
                if (f is IBackFragment fragment) {
                    handled = fragment.OnBackPressed();
                    if (handled) {
                        break;
                    }
                }
            }
            if (!handled) {
                router.GoBack();
            }
        }
    }
}

public static class AppCompatActivityExtensions
{
    public static void ShowSoftkeyboard(this AppCompatActivity activity) {
        var inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
        if (inputMethodManager != null && activity.CurrentFocus != null) {
            inputMethodManager.ShowSoftInput(activity.CurrentFocus, ShowFlags.Implicit);
        }
    }

    public static void HideSoftkeyboard(this AppCompatActivity activity) {
        var inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
        if (inputMethodManager != null && activity.CurrentFocus != null) {
            inputMethodManager.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, 0);
        }
    }
}