using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;

namespace NativeVyatkaAndroid
{
    public abstract class BaseAppCompatActivity : AppCompatActivity
    {      
        public void ShowSnack(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                using (var handler = new Handler(Application.MainLooper))
                {
                    handler.Post(() => Snackbar.Make(Window.DecorView, message, Snackbar.LengthShort).Show()); //FindViewById(Resource.Id.content_frame)
                }
            }
        }
    }
}

