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
    public enum QuestionType
    {
        ContinueWithoutGps
    }


    public class QuestionAlertDialog : DialogFragment
    {
        public static QuestionAlertDialog NewInstance(int message, int title, QuestionType type)
        {
            var dialog = new QuestionAlertDialog();
            var args = new Bundle();
            args.PutInt(MESSAGE, message);
            args.PutInt(TITLE, title);
            args.PutInt(TYPE, (int)type);
            dialog.Arguments = args;
            return dialog;
        }

        public static QuestionAlertDialog NewInstance(string message, string title, QuestionType type)
        {
            var dialog = new QuestionAlertDialog();  
            var args = new Bundle();
            args.PutString(MESSAGE, message);
            args.PutString(TITLE, title);
            args.PutInt(TYPE, (int)type);
            dialog.Arguments = args;
            return dialog;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            using (var adb = new AlertDialog.Builder(Activity))
            {
                adb.SetTitle(Title)
                    .SetMessage(Message)
                    .SetPositiveButton(Resource.String.dialog_ok, Positive)
                    .SetNegativeButton(Resource.String.dialog_cancel, Negative);
                return adb.Create();
            }
        }

        private void Positive(object sender, System.EventArgs e)
        {
            listener.OnDialogPositiveClick(Type);
            Dismiss();
        }

        private void Negative(object sender, System.EventArgs e)
        {
            listener.OnDialogNegitiveClick(Type);
            Dismiss();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            listener.OnDialogNegitiveClick(Type);
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
        private const string TYPE = "type";

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

        private QuestionType Type
        {
            get
            {
                return (QuestionType)Arguments.GetInt(TYPE);
            }
        }

        public interface IQuestionAlertDialogListener
        {
            void OnDialogPositiveClick(QuestionType type);
            void OnDialogNegitiveClick(QuestionType type);
        }
    }
}

