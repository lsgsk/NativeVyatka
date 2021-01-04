using Plugin.CurrentActivity;
using Android.Support.V4.App;
using Android.Support.V7.App;
using System;

namespace NativeVyatka
{
    public enum NavigationMenuIds
    {
        RecordsList,
        RecordsMap
    }

    public interface IRouter
    {
        void OpenLoginScreen();
        void OpenMainScreen();
        void OpenBurialEditScreen(BurialModel burial, Action callback);
        void GoBack();
    }

    public class Router : IRouter
    {
        private FragmentManager SupportFragmentManager => (CrossCurrentActivity.Current.Activity as AppCompatActivity).SupportFragmentManager;

        public void OpenLoginScreen() {
            string tag = typeof(LoginFlowFragment).ToString();
            var fragment = (SupportFragmentManager.FindFragmentByTag(tag) as LoginFlowFragment) ?? LoginFlowFragment.NewInstance();
            SupportFragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);    
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_content, fragment, tag)
                .SetTransition(FragmentTransaction.TransitFragmentOpen)
                .AddToBackStack(null)
                .Commit();
        }

        public void OpenMainScreen() {
            string tag = typeof(MainFlowFragment).ToString();
            var fragment = (SupportFragmentManager.FindFragmentByTag(tag) as MainFlowFragment) ?? MainFlowFragment.NewInstance();
            SupportFragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_content, fragment, tag)
                .SetTransition(FragmentTransaction.TransitFragmentOpen)
                .AddToBackStack(tag)
                .Commit();
        }

        public void OpenBurialEditScreen(BurialModel burial, Action callback) {
            string tag = typeof(BurialEditFlowFragment).ToString();
            var fragment = BurialEditFlowFragment.NewInstance(burial, callback);
            var supportFragmentManager = (CrossCurrentActivity.Current.Activity as AppCompatActivity).SupportFragmentManager;
            supportFragmentManager
                .BeginTransaction()
                .Add(Resource.Id.main_content, fragment, tag)
                .SetTransition(FragmentTransaction.TransitFragmentOpen)
                .AddToBackStack(tag)
                .Commit();
        }

        public void GoBack() {
            if (SupportFragmentManager.BackStackEntryCount > 1) {
                SupportFragmentManager.PopBackStack();
            }
            else {
                CrossCurrentActivity.Current.Activity.Finish();
            }
        }
    } 
}