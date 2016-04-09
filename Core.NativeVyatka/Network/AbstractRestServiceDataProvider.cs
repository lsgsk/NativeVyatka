using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Abstractions;
using Plugins;
using System.Linq;

namespace NativeVyatkaCore
{
    public interface IRestServiceDataProvider
    {
        Task<bool> UploadNewBurials(IEnumerable<BurialEntity> collection);
    }

    public abstract class AbstractRestServiceDataProvider :IRestServiceDataProvider
    {
        protected AbstractRestServiceDataProvider()
        {     
            
        }
        public async Task<bool> UploadNewBurials(IEnumerable<BurialEntity> collection)
        {
            try
            {
                var items = collection.Select(x => new ApiBurialEntity()
                    {
                        HashId = x.HashId,
                        Name = x.Name,
                        BirthTime = x.BirthTime,
                        DeathTime = x.DeathTime,
                        Desctiption = x.Desctiption,
                        Time = x.Time,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        Picture = CrossImageHelper.Current.ToByteArray(x.PicturePath)
                    });   
                await Task.Delay(1500);
                return true;
                //return await new RestServiceManager(ServiceUrl).UploadNewBurials(items);
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                return false;
            }
        }

        private readonly string ServiceUrl = "http://google.ru";
    }
}

