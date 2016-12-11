using Abstractions.Interfaces.Database.Tables;
using System.Collections.Generic;
using System;
using Abstractions.Models.DatabaseModels;
using Abstractions.Models.AppModels;
using Plugins;
using System.Linq;

namespace NativeVyatkaCore.Database
{
    public sealed partial class BurialDatabase : IBurialStorage
    {
        public int Count()
        {
            using (var conn = GetConnection())
            {
                return conn.Table<BurialEntity>().Count();
            }
        }

        public List<BurialModel> GetBurials()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return conn.Table<BurialEntity>().Select(x => new BurialModel(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
            return new List<BurialModel>();
        }

        public BurialModel GetBurial(string cloudId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var item = conn.Table<BurialEntity>().First(x => x.CloudId == cloudId);
                    return new BurialModel(item);
                }
            }
            catch(Exception ex)
            {
                iConsole.Error(ex);                
            }
            return BurialModel.Null;
        }
               
        public void InsertOrUpdateBurial(BurialModel item)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var burial = conn.Find<BurialEntity>(x => x.CloudId == item.CloudId);
                    if (burial != null)
                    {
                        var newBurial = item.ToBurialEntity();
                        newBurial.Id = burial.Id;
                        conn.Update(newBurial);
                    }
                    else
                    {
                        conn.Insert(item.ToBurialEntity());
                    }
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
        }


        public void DeleteBurial(string cloudId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var burial = conn.Table<BurialEntity>().Where(x => x.CloudId == cloudId).FirstOrDefault();
                    if (burial != null)
                    {
                        conn.Delete<BurialEntity>(burial.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
        }
    }
}
