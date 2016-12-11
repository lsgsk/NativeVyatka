using Abstractions.Models.AppModels;
using System.Collections.Generic;

namespace Abstractions.Interfaces.Database.Tables
{
    public interface IBurialStorage
    {
        int Count();
        List<BurialModel> GetBurials();
        BurialModel GetBurial(string cloudId);
        void InsertOrUpdateBurial(BurialModel item);
        void DeleteBurial(string cloudId);
    }
}
