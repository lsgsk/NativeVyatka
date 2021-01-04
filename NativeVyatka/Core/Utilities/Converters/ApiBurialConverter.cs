using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NativeVyatka
{
    public interface IApiBurialConverter
    {
        Task<ApiBurialToSend> Convert(BurialModel model);
        Task<string> Serialize(BurialModel model);
        IEnumerable<BurialModel> ParceJson(string json, string userHash);
    }

    public class ApiBurialConverter : IApiBurialConverter
    {
        public ApiBurialConverter(IBurialImageGuide guide)
        {
            this.mGuide = guide;
        }

        public async Task<ApiBurialToSend> Convert(BurialModel model)
        {
            var item = model.ToApiBurial();
            if (!model.PicturePath.StartsWith("http"))
            {
                var image = await ReadImage(model.PicturePath);
                item.Picture = System.Convert.ToBase64String(image);
            }
            return item;
        }

        public async Task<string> Serialize(BurialModel model)
        {
            var item = await Convert(model);
            return JsonConvert.SerializeObject(item);
        }

        private async Task<byte[]> ReadImage(string path)
        {
            try
            {
                return await mGuide.LoadFromFileSystemAsync(path);
            }
            catch (FileGuideException)
            {
                return new byte[0];
            }
        }

        public IEnumerable<BurialModel> ParceJson(string json, string userHash)
        {
            var collection = JsonConvert.DeserializeObject<IEnumerable<ApiBurialToReceive>>(json);
            return collection.Select(x => new BurialModel(x, userHash));
        }

        private IBurialImageGuide mGuide;
    }
}
