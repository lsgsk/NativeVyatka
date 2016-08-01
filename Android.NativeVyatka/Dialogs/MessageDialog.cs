using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using Android.App;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using NativeVyatkaCore;

namespace NativeVyatkaAndroid
{
    public sealed class MessageDialog : DialogFragment
    {
        public static MessageDialog NewInstance(int message, int title, DialogType type = DialogType.Null )
        {
            var dialog = new MessageDialog();    
            var args = new Bundle();
            args.PutInt(MESSAGE, message);
            args.PutInt(TITLE, title);
            args.PutInt(TYPE, (int)type);
            dialog.Arguments = args;
            return dialog;
        }

        public static MessageDialog NewInstance(string message, string title)
        {
            var dialog = new MessageDialog();  
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
                   .SetPositiveButton(Resource.String.dialog_ok, (s, e) => meListener.OnDialogPositiveClick(Type));      
                return adb.Create();
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            meListener = (IMessageDialogListener)context;
        }

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

        private DialogType Type
        {
            get
            {
                return (DialogType)Arguments.GetInt(TYPE);
            }
        }

        private IMessageDialogListener meListener;
        public const string MessageDialogTag = "MessageDialogTag";
        private const string MESSAGE = "message";
        private const string TITLE = "title";
        private const string TYPE = "type";
    }

    public interface IMessageDialogListener
    {
        void OnDialogPositiveClick(DialogType type);
    }
}

