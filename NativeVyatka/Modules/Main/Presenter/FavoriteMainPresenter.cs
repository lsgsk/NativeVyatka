using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace NativeVyatka
{
    public interface IFavoritesPresenter : IMainRecordsPresenter
    {
    }

    public partial class MainPresenter: IFavoritesPresenter
    {
    }
}
#nullable restore