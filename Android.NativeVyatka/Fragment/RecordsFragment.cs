﻿using Android.OS;
using Android.Views;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using System.Collections.Generic;
using Abstractions.Models.AppModels;
using Android.Content;
using Abstractions.Interfaces.Controllers;

namespace NativeVyatkaAndroid
{
    public class RecordsFragment : ProgressFragment
    {
        public static RecordsFragment NewInstance()
        {
            var fragment = new RecordsFragment();
            return fragment;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            mController = (context as MainActivity).mController;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {            
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
            mRecyclerView.AddItemDecoration(new SimpleDividerItemDecoration(Activity));
            mAdapter = new BaseRecyclerViewAdapter<BurialModel, BurialRecordViewHolder>(Activity, new List<BurialModel>(), Resource.Layout.Item_BurialRecord);
            mAdapter.ItemClick += BurialRecordItemClick;
            mRecyclerView.SetAdapter(mAdapter);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetEmptyText(Resource.String.null_content);
            DisplayVisitors();
        }

        protected void DisplayVisitors()
        {
            SetContentShown(false);
            var items = mController.GetBurials();
            mAdapter.UpdateItems(items);
            SetContentEmpty(false);
            SetContentShown(true);
        }

        protected async Task ObtainData(bool force = false)
        {
            try
            {
                SetContentShown(false);
                var items = mController.GetBurials();
                mAdapter.UpdateItems(items);
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

        private void BurialRecordItemClick (object sender, BaseEventArgs<BurialModel> e)
        {
            mController.DisplayBurial(e.Item);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {      
            base.OnPrepareOptionsMenu(menu);
            //menu.FindItem(Resource.Id.action_filter).SetVisible(false);
            //menu.FindItem(Resource.Id.action_search).SetVisible(true);             
        }
        private RecyclerView mRecyclerView;
        private BaseRecyclerViewAdapter<BurialModel, BurialRecordViewHolder> mAdapter;
        private View mContentView;
        public const string RecordsFragmentTag = "RecordsFragmentTag";

        private IMainRecordsController mController;
    }
}

