using Android.Widget;
using Android.Views;
using Java.IO;
using Abstractions.Models.AppModels;
using Square.Picasso;

namespace NativeVyatkaAndroid
{
    public class BurialRecordViewHolder : IBindViewHolder<BurialModel>
    {
        public TextView tvName { get; private set; }
        public TextView tvDescription { get; private set; }
        public ImageView imgImage { get; private set; }
        public View vIsSended { get; private set; }
       
        public void FindViews(View view)
        {
            tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            tvDescription = view.FindViewById<TextView>(Resource.Id.tvDescription);
            imgImage = view.FindViewById<ImageView>(Resource.Id.imgImage);
            vIsSended = view.FindViewById<View>(Resource.Id.vIsSended);
        }

        public void BindItem(BurialModel burial)
        {       
            tvName.Text = $"{burial.Name} {burial.Surname} {burial.Patronymic}";
            tvDescription.Text = burial.Desctiption;
            Picasso.With(imgImage.Context).Load(new File(burial.PicturePath)).Resize(100, 100).CenterCrop().Into(imgImage);            
        }
    }
}

