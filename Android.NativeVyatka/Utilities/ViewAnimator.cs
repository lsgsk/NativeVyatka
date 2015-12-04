using System;
using Android.Animation;
using System.Threading;

namespace NativeVyatkaAndroid
{
    public class ViewAnimator : Java.Lang.Object, Animator.IAnimatorListener
    {
        public void OnAnimationCancel(Animator animation)
        {
            
        }

        public EventHandler AnimationEndEvent;

        public void OnAnimationEnd(Animator animation)
        {
            var temp = Volatile.Read(ref AnimationEndEvent);
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationStart(Animator animation)
        {
        }
    }
}

