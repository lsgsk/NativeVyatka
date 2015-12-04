using Android.Widget;
using Android.Views;
using NativeVyatkaCore;
using Android.App;
using IT.Sephiroth.Android.Library.Picasso;
using Java.IO;

namespace NativeVyatkaAndroid
{
    public class BurialRecordViewHolder : IBindViewHolder<BurialEntity>
    {
        public TextView tvName { get; private set; }
        public TextView tvDescription { get; private set; }
        public ImageView imgImage { get; private set; }
        public View vIsSended { get; private set; }
        private PhotoStorageManager mPsm = new PhotoStorageManager(Application.Context);

        public void FindViews(View view)
        {
            tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            tvDescription = view.FindViewById<TextView>(Resource.Id.tvDescription);
            imgImage = view.FindViewById<ImageView>(Resource.Id.imgImage);
            vIsSended = view.FindViewById<View>(Resource.Id.vIsSended);
        }

        public void BindItem(BurialEntity item)
        {       
            tvName.Text = item.Name;
            tvDescription.Text = item.Desctiption;
            vIsSended.Visibility = item.IsSended ? ViewStates.Invisible : ViewStates.Visible;
            if (item.PicturePath != null)
            {
                Picasso.With(imgImage.Context).Load(new File(Application.Context.FilesDir.AbsolutePath + "/" +item.PicturePath)).Resize(100, 100).CenterCrop().Into(imgImage);
            }
        }
    }
}

