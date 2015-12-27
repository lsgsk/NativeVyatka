
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
using System.Threading.Tasks;
using Abstractions;

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
            mBurialManager = MainApplication.Container.Resolve<IBurialsManager>();
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
            Refresher.Refresh += async (sender, e) => await ObtainData(true); 
            RepeatClick += async (sender, e) => await ObtainData(true);
            mRecyclerView = mContentView.FindViewById<RecyclerView>(Resource.Id.rvRecordsList);
            mRecyclerView.HasFixedSize = true;
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
        }

        public override void OnDestroyView()
        {
            mLoadRecordTokenSource.Cancel();
            base.OnDestroyView();
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetEmptyText(Resource.String.null_content);
            await ObtainData(savedInstanceState == null);
        }

        protected async Task ObtainData(bool force = false)
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
                SetContentEmpty(true);
                SetContentShown(true);  
            }
            finally
            {
                Refresher.Refreshing = false;
            }
        }

        private void BurialRecordItemClick (object sender, BaseEventArgs<BurialEntity> e)
        {
            var intent = new Intent(Activity, typeof(BurialDetailActivity));
            intent.PutExtra(BurialDetailActivity.BURIAL_ID, e.Item.Id);
            StartActivityForResult(intent, MainActivity.OPEN_BURIAL);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {           
            menu.FindItem(Resource.Id.action_filter).SetVisible(false);
            menu.FindItem(Resource.Id.action_search).SetVisible(true);               
            base.OnPrepareOptionsMenu(menu);
        }
        private IBurialsManager mBurialManager;
        private RecyclerView mRecyclerView;
        private View mContentView;
        public const string RecordsFragmentTag = "RecordsFragmentTag";

        private CancellationTokenSource mLoadRecordTokenSource;
    }
}

