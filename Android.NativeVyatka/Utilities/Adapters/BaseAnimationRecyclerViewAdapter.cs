using System;
using Android.Content;
using System.Collections.Generic;

namespace NativeVyatkaAndroid
{
    public class BaseAnimationRecyclerViewAdapter<T,K> : BaseRecyclerViewAdapter<T,K> where T : class where K : IBindViewHolder<T>, new()
    {
        public BaseAnimationRecyclerViewAdapter(Context context, IList<T> items, int layout) : base(context, items, layout)
        {
            //http://stackoverflow.com/questions/30398247/how-to-filter-a-recyclerview-with-a-searchview
        }

        public T RemoveItem(int position)
        {
            var item = mItems[position];
            mItems.RemoveAt(position);
            NotifyItemRemoved(position);
            return item;
        }

        public void AddItem(int position, T item)
        {
            mItems.Insert(position, item);
            NotifyItemInserted(position);
        }

        public void MoveItem(int fromPosition, int toPosition)
        {
            var model = mItems[fromPosition];
            mItems.RemoveAt(fromPosition);
            mItems.Insert(toPosition, model);
            NotifyItemMoved(fromPosition, toPosition);
        }

        public void AnimateTo(IList<T> models)
        {
            ApplyAndAnimateRemovals(models);
            ApplyAndAnimateAdditions(models);
            ApplyAndAnimateMovedItems(models);
        }

        private void ApplyAndAnimateRemovals(ICollection<T> newModels)
        {
            for (int i = mItems.Count - 1; i >= 0; i--)
            {
                var model = mItems[i];
                if (!newModels.Contains(model))
                {
                    RemoveItem(i);
                }
            }
        }

        private void ApplyAndAnimateAdditions(IList<T> newModels)
        {
            for (int i = 0, count = newModels.Count; i < count; i++)
            {
                var model = newModels[i];
                if (!mItems.Contains(model))
                {
                    AddItem(i, model);
                }
            }
        }

        private void ApplyAndAnimateMovedItems(IList<T> newModels)
        {
            for (int toPosition = newModels.Count - 1; toPosition >= 0; toPosition--)
            {
                var model = newModels[toPosition];
                var fromPosition = mItems.IndexOf(model);
                if (fromPosition >= 0 && fromPosition != toPosition)
                {
                    MoveItem(fromPosition, toPosition);
                }
            }
        }
    }
}

