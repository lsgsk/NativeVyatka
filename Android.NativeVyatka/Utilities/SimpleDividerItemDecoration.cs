using System;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Content;
using Android.Support.V4.Content;

namespace NativeVyatkaAndroid
{
    public class SimpleDividerItemDecoration :  RecyclerView.ItemDecoration
    {
        private readonly Drawable mDivider;

        public SimpleDividerItemDecoration(Context context)
        {
            mDivider = ContextCompat.GetDrawable(context, Resource.Drawable.line_divider);
        }

        public override void OnDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            int left = parent.PaddingLeft;
            int right = parent.Width - parent.PaddingRight;
            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChildAt(i);
                var prms = (RecyclerView.LayoutParams)child.LayoutParameters;
                int top = child.Bottom + prms.BottomMargin;
                int bottom = top + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }
    }
}

