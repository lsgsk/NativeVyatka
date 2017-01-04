using Android.OS;
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
           return new RecordsFragment();
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
            Refresher.Refresh += async (sender, e) => await ObtainData(); 
            RepeatClick += async (sender, e) => await ObtainData();
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
            DisplayRecords();
        }

        protected void DisplayRecords()
        {
            SetContentShown(false);
            var items = mController.GetBurials();
            mAdapter.UpdateItems(items);
            SetContentEmpty(items.Count == 0);
            SetContentShown(true);
        }

        protected async Task ObtainData()
        {
            await mController.ForceSyncBurials();
            DisplayRecords();
            Refresher.Refreshing = false;
        }

        private void BurialRecordItemClick (object sender, BaseEventArgs<BurialModel> e)
        {
            mController.DisplayBurial(e.Item);
        }      

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_sync:
                    Task.Run(ObtainData);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private RecyclerView mRecyclerView;
        private BaseRecyclerViewAdapter<BurialModel, BurialRecordViewHolder> mAdapter;
        private View mContentView;
        public const string RecordsFragmentTag = "RecordsFragmentTag";
        private IMainRecordsController mController;
    }
}

