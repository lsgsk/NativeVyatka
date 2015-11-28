
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Support.V7.Widget;
using NativeVyatkaCore;
using Microsoft.Practices.Unity;

namespace NativeVyatkaAndroid
{
    public class RecordsFragment : ProgressFragment
    {
        public static RecordsFragment NewInstance()
        {
            var fragment = new RecordsFragment();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
            mBurialManager = AppApplication.Container.Resolve<IBurialsManager>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {            
            mLoadRecordTokenSource = new CancellationTokenSource();
            mContentView = inflater.Inflate(Resource.Layout.Fragment_Records, null);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            Refresher.Refresh += (sender, e) => ObtainData(true); 
            RepeatClick += (sender, e) => ObtainData(true);
            mRecyclerView = mContentView.FindViewById<RecyclerView>(Resource.Id.rvRecordsList);
            mRecyclerView.HasFixedSize = true;
            //mRecyclerView.AddItemDecoration(new DividerItemDecoration(this));
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
        }

        public override void OnDestroyView()
        {
            mLoadRecordTokenSource.Cancel();
            base.OnDestroyView();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetEmptyText(Resource.String.null_content);
            ObtainData(savedInstanceState == null);
        }

        protected async void ObtainData(bool force = false)
        {
            try
            {
                SetContentShown(false);
                var items = await mBurialManager.GetAllBurials(mLoadRecordTokenSource.Token);
                var adapter = new BaseRecyclerViewAdapter<BurialEntity, BurialRecordViewHolder>(Activity, items, Resource.Layout.Item_BurialRecord);
                adapter.ItemClick += BurialRecordItemClick;
                mRecyclerView.SetAdapter(adapter);
                SetContentEmpty(false);
                SetContentShown(true); 
            }
            catch
            {
                ShowErrorAction();
            }
        }

        private void BurialRecordItemClick (object sender, BaseEventArgs<BurialEntity> e)
        {
            
        }

        private void ShowErrorAction()
        {
            SetContentEmpty(true);
            SetContentShown(true);  
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {           
            menu.FindItem(Resource.Id.action_filter).SetVisible(true);
            menu.FindItem(Resource.Id.action_search).SetVisible(false);               
            base.OnPrepareOptionsMenu(menu);
        }
        private IBurialsManager mBurialManager;
        private RecyclerView mRecyclerView;
        private View mContentView;
        public const string RecordsFragmentTag = "RecordsFragmentTag";

        private CancellationTokenSource mLoadRecordTokenSource;
    }
}

