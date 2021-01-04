using System;

namespace NativeVyatka
{
    public class FavoritesFragment : RecordsFragment
    {
        private readonly IFavoritesPresenter presenter;

        public static FavoritesFragment NewInstance(IFavoritesPresenter presenter) {
            return new FavoritesFragment(presenter) {
                RetainInstance = true
            };
        }

        public FavoritesFragment(IFavoritesPresenter presenter) : base(presenter) {
            this.presenter = presenter;
        }

        public override Func<BurialModel, bool> FilterPredicate => (item) => item.Favorite;
    }
}
