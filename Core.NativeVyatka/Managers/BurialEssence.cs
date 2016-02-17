using System;
using System.Threading.Tasks;
using System.IO;
using Plugin.Geolocator;
using Abstractions;
using Plugin.Geolocator.Abstractions;
using Plugins;

namespace NativeVyatkaCore
{
    public class BurialEssence : IBurialEssence
    {
        private BurialEssence()
        {
        }

        private async Task<BurialEssence> InitializeAsync(byte[] image)
        {
            Item = new BurialEntity();
            var name = Path.GetRandomFileName() + ".png";
            await mImageFactor.SaveImageToFileSystemAsync(image, name);
            await InitDefaults(name);
            await mBurialsManager.InsertBurial(Item);
            return this;
        }

        private async Task<BurialEssence> InitializeAsync(int id)
        {
            Item = await mBurialsManager.GetBurial(id);
            return this;
        }

        public static Task<BurialEssence> CreateAsync(byte[] image, IBurialsManager burialsManager, IImageFactor imageFactor)
        {
            var ret = new BurialEssence();
            ret.mBurialsManager = burialsManager;
            ret.mImageFactor = imageFactor;
            return ret.InitializeAsync(image);
        }

        public static Task<BurialEssence> GetAsync(int id, IBurialsManager burialsManager, IImageFactor imageFactor)
        {
            var ret = new BurialEssence();
            ret.mBurialsManager = burialsManager;
            ret.mImageFactor = imageFactor;
            return ret.InitializeAsync(id);
        }

        private async Task InitDefaults(string name)
        {
            var position = await GetGeoPosition();
            Item.HashId = Guid.NewGuid().ToString();
            Item.Time = DateTime.UtcNow;
            Item.Latitude = position.Latitude;      
            Item.Longitude = position.Longitude;
            Item.PicturePath = mImageFactor.GetImagePath(name);
            Item.IsSended = false;           
        }

        private static async Task<Position> GetGeoPosition()
        {
            try
            {
                return await CrossGeolocator.Current.GetPositionAsync(5000);
            }
            catch
            {
                return new Position();
            }
        }

        private static async Task<string> GetAdress(Position position)
        {
            if (position != null)
            {
                try
                {
                    return await CrossFetchAddress.Current.GetAdress(position.Latitude, position.Longitude);
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public BurialEntity Item { get; private set; }

        private IBurialsManager mBurialsManager { get; set; }

        private IImageFactor mImageFactor { get; set; }
    }
}

