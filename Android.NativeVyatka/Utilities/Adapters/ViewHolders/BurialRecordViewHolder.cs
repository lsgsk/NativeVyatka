using Android.Widget;
using Android.Views;
using Android.Content;
using System;
using NativeVyatkaCore;

namespace NativeVyatkaAndroid
{
    public class BurialRecordViewHolder : IBindViewHolder<BurialEntity>
    {
        public TextView tvName { get; private set; }
        public TextView tvDescription { get; private set; }
        public  ImageView imgImage { get; private set; }

        public void FindViews(View view)
        {
            tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            tvDescription = view.FindViewById<TextView>(Resource.Id.tvDescription);
            imgImage = view.FindViewById<ImageView>(Resource.Id.imgImage);
        }

        public void BindItem(BurialEntity item)
        {            
            tvName.Text = item.Name;
            tvDescription.Text = item.Desctiption;
        }
    }
}

