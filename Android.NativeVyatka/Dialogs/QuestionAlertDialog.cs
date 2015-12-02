using DialogFragment = Android.Support.V4.App.DialogFragment;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace NativeVyatkaAndroid
{
    public class QuestionAlertDialog : DialogFragment
    {
        public static QuestionAlertDialog NewInstance(int message, int title)
        {
            var dialog = new QuestionAlertDialog();
            var args = new Bundle();
            args.PutInt(MESSAGE, message);
            args.PutInt(TITLE, title);
            dialog.Arguments = args;
            return dialog;
        }

        public static QuestionAlertDialog NewInstance(string message, string title)
        {
            var dialog = new QuestionAlertDialog();  
            var args = new Bundle();
            args.PutString(MESSAGE, message);
            args.PutString(TITLE, title);
            dialog.Arguments = args;
            return dialog;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            using (var adb = new AlertDialog.Builder(Activity))
            {
                adb.SetTitle(Title)
                    .SetMessage(Message)
                    .SetPositiveButton(Resource.String.dialog_ok, (s, e) => listener.OnDialogPositiveClick())
                    .SetNegativeButton(Resource.String.dialog_cancel, (s, e) => listener.OnDialogNegitiveClick());
                return adb.Create();
            }
        }

        private void Positive(object sender, System.EventArgs e)
        {
            listener.OnDialogPositiveClick();
            Dismiss();
        }

        private void Negative(object sender, System.EventArgs e)
        {
            listener.OnDialogNegitiveClick();
            Dismiss();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            listener.OnDialogNegitiveClick();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            listener = (IQuestionAlertDialogListener)context;
        }

        private IQuestionAlertDialogListener listener;
        public const string QuestionAlertDialogTag = "QuestionAlertDialogTag";
        private const string MESSAGE = "message";
        private const string TITLE = "title";
        private string Message
        {
            get
            {
                return  Arguments.GetString(MESSAGE) ?? Resources.GetString(Arguments.GetInt(MESSAGE));
            }
        }
        private string Title
        {
            get
            {
                return Arguments.GetString(TITLE) ?? Resources.GetString(Arguments.GetInt(TITLE));
            }
        }
        public interface IQuestionAlertDialogListener
        {
            void OnDialogPositiveClick();
            void OnDialogNegitiveClick();
        }
    }   
}

