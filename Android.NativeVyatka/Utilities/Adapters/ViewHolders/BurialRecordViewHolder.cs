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
            var name = $"{burial.Surname} {burial.Name} {burial.Patronymic}";
            tvName.Text = string.IsNullOrWhiteSpace(name) ? "Неизвестное захоронение" : name;
            tvDescription.Text = string.IsNullOrEmpty(burial.Description) ? "Без описания" : burial.Description;
            if (!string.IsNullOrEmpty(burial.PicturePath))
            {
                var picasso = Picasso.With(imgImage.Context);
                var creator = (burial.PicturePath.StartsWith("http")) 
                    ? picasso.Load(burial.PicturePath) 
                    : picasso.Load(new File(burial.PicturePath));
                creator.Resize(100, 100).CenterCrop().Into(imgImage);
            }
            vIsSended.Visibility = burial.Updated ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}

