using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace NativeVyatkaAndroid
{
    public class SquareFrameLayout : FrameLayout
    {
        public SquareFrameLayout(Context context) : base(context)
        {
        }

        public SquareFrameLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);

            if (width > (int)((mScale * height) + 0.5))
            {
                width = (int)((mScale * height) + 0.5);
            }
            else
            {
                height = (int)((width / mScale) + 0.5);
            }
            base.OnMeasure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
        }
        // Desired width-to-height ratio - 1.0 for square
        private readonly double mScale = 1.0;
    }
}

