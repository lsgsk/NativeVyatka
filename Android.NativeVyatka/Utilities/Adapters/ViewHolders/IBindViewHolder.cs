using Android.Views;

namespace NativeVyatkaAndroid
{
    public interface IBindViewHolder<T>
    {        
        void FindViews(View view);
        void BindItem(T item);
    }
}

