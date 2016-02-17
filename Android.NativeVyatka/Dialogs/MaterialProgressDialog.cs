using DialogFragment = Android.Support.V4.App.DialogFragment;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.App;
using Android.OS;

namespace NativeVyatkaAndroid
{
    public class MaterialProgressDialog : DialogFragment
    {
        public static MaterialProgressDialog NewInstance(bool cancel = false)
        {
            var dialog = new MaterialProgressDialog();
            var args = new Bundle();
            args.PutBoolean(CANCEL, cancel);
            return dialog;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Cancelable = Arguments?.GetBoolean(CANCEL) ?? true;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = new ProgressDialog(Activity);
            dialog.SetMessage(Activity.GetString(Resource.String.dialogs_loadings));
            dialog.Indeterminate = true;
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.SetCanceledOnTouchOutside(false);
            return dialog;
        }

        private const string CANCEL = "cancel";
        public const string MaterialProgressDialogTag = "MaterialProgressDialogTag";
    }
}

