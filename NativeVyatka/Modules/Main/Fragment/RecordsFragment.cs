using Android.OS;
using Android.Views;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System;

namespace NativeVyatka
{
    public class RecordsFragment : ProgressFragment, IRecordsObserver
    {
        private readonly IMainRecordsPresenter presenter;
        internal RecyclerView recyclerView;
        internal BaseRecyclerViewAdapter<BurialModel, BurialRecordViewHolder> adapter;
        internal View contentView;

        public static RecordsFragment NewInstance(IMainRecordsPresenter presenter) {
            return new RecordsFragment(presenter) {
                RetainInstance = true
            };
        }

        public RecordsFragment(IMainRecordsPresenter presenter) {
            this.presenter = presenter;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            this.contentView = inflater.Inflate(Resource.Layout.Fragment_Records, null);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            Refresher.Refresh += (sender, e) => ObtainData();
            recyclerView = contentView.FindViewById<RecyclerView>(Resource.Id.rvRecordsList);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
            recyclerView.AddItemDecoration(new SimpleDividerItemDecoration(Activity));
            adapter = new BaseRecyclerViewAdapter<BurialModel, BurialRecordViewHolder>(Activity, new List<BurialModel>(), Resource.Layout.Item_BurialRecord);
            adapter.ItemClick += (sender, e) => presenter.DisplayBurial(e.Item);
            recyclerView.SetAdapter(adapter);
            Refresher.Refreshing = false;
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(contentView);
            presenter.DisplayBurials();
        }

        public override void OnDestroyView() {
            base.OnDestroyView();
            this.presenter.RemoveObserver(this);
        }

        public virtual void OnRecordsLoaded(List<BurialModel> burials) {
            adapter.UpdateItems(burials);
            SetContentEmpty(false);
            SetContentShown(true);
            Refresher.Refreshing = false;
        }

        public void OnRecordsFailed(string message) {
            SetEmptyText(message);
            SetContentEmpty(true);
            SetContentShown(true);
            Refresher.Refreshing = false;
        }

        protected void ObtainData() {
            presenter.SyncBurials();
        }

        public virtual Func<BurialModel, bool> FilterPredicate => (item) => true;
    }
}

