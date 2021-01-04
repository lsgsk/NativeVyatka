using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Views.Animations;
using Android.Support.V4.App;

namespace NativeVyatka
{
    public class ProgressFragment : Fragment
    {
        private View mProgressContainer;
        private View mContentContainer;
        private View mContentView;
        private View mEmptyView;
        private bool mContentShown;
        private bool mIsContentEmpty;
        public event EventHandler RepeatClick;
        public SwipeRefreshLayout Refresher;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Fragment_Progress, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            EnsureContent();
        }

        public override void OnDestroyView()
        {
            mContentShown = false;
            mIsContentEmpty = false;
            mProgressContainer = mContentContainer = mContentView = mEmptyView = null;
            base.OnDestroyView();
        }

        public View ContentView
        {
            get
            {
                return mContentView;
            }
        }

        public void SetContentView(int layoutResId)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(Activity);
            View contentView = layoutInflater.Inflate(layoutResId, null);
            SetContentView(contentView);
        }

        public void SetContentView(View view)
        {
            EnsureContent();
            if (view == null)
            {
                throw new Exception("Content view can't be null");
            }
            var viewGroup = mContentContainer as ViewGroup;
            if (viewGroup != null)
            {
                ViewGroup contentContainer = viewGroup;
                if (mContentView == null)
                {
                    contentContainer.AddView(view);
                }
                else
                {
                    int index = contentContainer.IndexOfChild(mContentView);
                    // replace content view
                    contentContainer.RemoveView(mContentView);
                    contentContainer.AddView(view, index);
                }
                mContentView = view;
            }
            else
            {
                throw new Exception("Can't be used with a custom content view");
            }
        }

        public void SetEmptyText(int resId)
        {
            SetEmptyText(GetString(resId));
        }

        public void SetEmptyText(string text)
        {
            EnsureContent();
            var textView = mEmptyView as TextView;
            if (textView != null)
            {
                textView.Text = text;
            }
            else
            {
                throw new Exception("Can't be used with a custom content view");
            }
        }

        public void SetContentShown(bool shown)
        {
            SetContentShown(shown, true);
        }


        public void SetContentShownNoAnimation(bool shown)
        {
            SetContentShown(shown, false);
        }

        private void SetContentShown(bool shown, bool animate)
        {
            EnsureContent();
            if (mContentShown == shown)
            {
                return;
            }
            mContentShown = shown;
            if (shown)
            {
                if (animate)
                {
                    mProgressContainer.StartAnimation(AnimationUtils.LoadAnimation(Activity, Android.Resource.Animation.FadeOut));
                    mContentContainer.StartAnimation(AnimationUtils.LoadAnimation(Activity, Android.Resource.Animation.FadeIn));
                }
                else
                {
                    mProgressContainer.ClearAnimation();
                    mContentContainer.ClearAnimation();
                }
                mProgressContainer.Visibility = ViewStates.Gone;
                mContentContainer.Visibility = ViewStates.Visible;
            }
            else
            {
                if (animate)
                {
                    mProgressContainer.StartAnimation(AnimationUtils.LoadAnimation(Activity, Android.Resource.Animation.FadeIn));
                    mContentContainer.StartAnimation(AnimationUtils.LoadAnimation(Activity,Android.Resource.Animation.FadeOut));
                }
                else
                {
                    mProgressContainer.ClearAnimation();
                    mContentContainer.ClearAnimation();
                }

                mProgressContainer.Visibility = ViewStates.Visible;
                mContentContainer.Visibility = ViewStates.Gone;
            }
        }

        public bool IsContentEmpty
        {
            get
            {
                return mIsContentEmpty;
            }
        }

        public void SetContentEmpty(bool isEmpty)
        {
            EnsureContent();
            if (mContentView == null)
            {
                throw new Exception("Content view must be initialized before");
            }
            if (isEmpty)
            {
                mEmptyView.Visibility = ViewStates.Visible;  
                mContentView.Visibility = ViewStates.Gone;
            }
            else
            {
                mEmptyView.Visibility = ViewStates.Gone;
                mContentView.Visibility = ViewStates.Visible;
            }
            mIsContentEmpty = isEmpty;
        }

        private void EnsureContent()
        {
            if (mContentContainer != null && mProgressContainer != null)
            {
                return;
            }
            View root = View;
            if (root == null)
            {
                throw new Exception("Content view not yet created");
            }
            mProgressContainer = root.FindViewById(Resource.Id.progress_container);
            if (mProgressContainer == null)
            {
                throw new Exception("Your content must have a ViewGroup whose id attribute is 'R.id.progress_container'");
            }
            mContentContainer = root.FindViewById(Resource.Id.content_container);
            if (mContentContainer == null)
            {
                throw new Exception("Your content must have a ViewGroup whose id attribute is 'R.id.content_container'");
            }
            mEmptyView = root.FindViewById(Android.Resource.Id.Empty);
            mEmptyView.Click += (sender, e) => 
                {
                    if(RepeatClick != null) 
                        RepeatClick(this, EventArgs.Empty);
                };
            if (mEmptyView != null)
            {
                mEmptyView.Visibility = ViewStates.Gone;
            }
            mContentShown = true;
            // We are starting without a content, so assume we won't
            // have our data right away and start with the progress indicator.
            if (mContentView == null)
            {
                SetContentShown(false, false);
            }
            Refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            //Refresher.Setcol.SetColorScheme(Android.Resource.Color.Blu, Resource.Color.xam_purple, Resource.Color.xam_gray, Resource.Color.xam_green);
        }
    }
}

