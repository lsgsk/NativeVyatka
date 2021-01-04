using System;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using System.Threading;

namespace NativeVyatka
{
    public interface IBindViewHolder<T>
    {
        void FindViews(View view);
        void BindItem(T item);
    }

    public class BaseEventArgs<T> : EventArgs
    {
        public BaseEventArgs() {
        }
        public int Position { get; set; }
        public T Item { get; set; }
    }

    public class BaseRecyclerViewAdapter<T, K> : RecyclerView.Adapter where K : IBindViewHolder<T>, new()
    {
        public event EventHandler<BaseEventArgs<T>> ItemClick;
        protected readonly LayoutInflater mInflater;
        protected readonly IList<T> mItems;
        protected readonly int mLayout;

        public BaseRecyclerViewAdapter(Context context, IList<T> items, int layout) {
            this.mInflater = LayoutInflater.From(context);
            this.mItems = new List<T>(items);
            this.mLayout = layout;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            var view = mInflater.Inflate(mLayout, parent, false);
            var bindHolder = new K();
            return new ViewHolder<T, K>(view, bindHolder, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            var item = mItems[position];
            (holder as ViewHolder<T, K>).BindItem(item);
        }

        public override int ItemCount
        {
            get
            {
                return mItems.Count;
            }
        }

        protected void OnClick(int position) {
            var temp = Volatile.Read(ref ItemClick);
            if (temp != null) {
                var item = new BaseEventArgs<T>() { Item = GetItem(position), Position = position };
                temp(this, item);
            }
        }

        public T GetItem(int position) {
            return mItems[position];
        }

        private class ViewHolder<S, M> : RecyclerView.ViewHolder where M : IBindViewHolder<S>
        {
            public ViewHolder(View itemView, M bindHolder, Action<int> listener) : base(itemView) {
                mBindHolder = bindHolder;
                mBindHolder.FindViews(itemView);
                itemView.Click += (sender, e) => listener(AdapterPosition);
            }

            public void BindItem(S item) {
                mBindHolder.BindItem(item);
            }

            protected readonly M mBindHolder;
        }

        public void UpdateItems(IList<T> newItems) {
            mItems.Clear();
            foreach (var item in newItems) {
                mItems.Add(item);
            }
            NotifyDataSetChanged();
        }
    }
}

