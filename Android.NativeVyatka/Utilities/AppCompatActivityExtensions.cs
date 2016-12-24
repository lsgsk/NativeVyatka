using Android.Support.V7.App;
using Android.Views.InputMethods;
using Android.Content;

namespace NativeVyatkaAndroid
{
    public static class AppCompatActivityExtensions
    {
        public static void ShowSoftkeyboard(this AppCompatActivity activity)
        {
            var inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            if (inputMethodManager != null && activity.CurrentFocus != null)
            {
                inputMethodManager.ShowSoftInput(activity.CurrentFocus, ShowFlags.Implicit);
            }
        }

        public static void HideSoftkeyboard(this AppCompatActivity activity)
        {
            var inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            if (inputMethodManager != null && activity.CurrentFocus != null)
            {                
                inputMethodManager.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, 0);
            }
        }
    }
}

