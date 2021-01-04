using System.Collections.Generic;
using System;
using System.Linq;

namespace NativeVyatka
{
    public interface IBurialStorage
    {
        int Count();
        List<BurialModel> GetBurials();
        List<BurialModel> GetNotSyncBurials();
        BurialModel GetBurial(string cloudId);
        void InsertOrUpdateBurial(BurialModel item);
        void DeleteBurial(string cloudId);
    }

    public sealed partial class BurialDatabase : IBurialStorage
    {
        public int Count()
        {
            try {
                using var conn = GetConnection();
                return conn.Table<BurialEntity>().Count();
            } catch (Exception ex) {
                iConsole.Error(ex);
                return 0;
            }
        }

        public List<BurialModel> GetBurials()
        {
            try {
                using var conn = GetConnection();
                return conn.Table<BurialEntity>().ToList().Select(x => new BurialModel(x)).OrderByDescending(x => x.RecordTime).ToList();
            } catch (Exception ex) {
                iConsole.Error(ex);
            }
            return new List<BurialModel>();
        }

        public List<BurialModel> GetNotSyncBurials()
        {
            try {
                using var conn = GetConnection();
                return conn.Table<BurialEntity>().Where(x => x.Updated == false).ToList().Select(x => new BurialModel(x)).ToList();
            } catch (Exception ex) {
                iConsole.Error(ex);
            }
            return new List<BurialModel>();
        }

        public BurialModel GetBurial(string cloudId)
        {
            try {
                using var conn = GetConnection();
                var item = conn.Table<BurialEntity>().First(x => x.CloudId == cloudId);
                return new BurialModel(item);
            } catch (Exception ex) {
                iConsole.Error(ex);
            }
            return BurialModel.Null;
        }

        public void InsertOrUpdateBurial(BurialModel item)
        {
            try {
                using var conn = GetConnection();
                var burial = conn.Find<BurialEntity>(x => x.CloudId == item.CloudId);
                if (burial != null)
                {
                    conn.Update(item.ToBurialEntity());
                }
                else
                {
                    conn.Insert(item.ToBurialEntity());
                }
            } catch (Exception ex) {
                iConsole.Error(ex);
            }
        }

        public void DeleteBurial(string cloudId)
        {
            try {
                using var conn = GetConnection();
                var burial = conn.Table<BurialEntity>().Where(x => x.CloudId == cloudId).FirstOrDefault();
                if (burial != null)
                {
                    conn.Delete<BurialEntity>(burial.CloudId);
                }
            } catch (Exception ex) {
                iConsole.Error(ex);
            }
        }
    }
}
