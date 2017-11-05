﻿using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Utilities;
using Abstractions.Models.AppModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Utilities
{
    public class ApiBurialConverter : IApiBurialConverter
    {
        public ApiBurialConverter(IBurialImageGuide guide)
        {
            this.mGuide = guide;
        }

        public async Task<ApiBurial> Convert(BurialModel model)
        {
            var item = model.ToApiBurial();
            var image = await ReadImage(model.PicturePath);
            item.PictureUrl = System.Convert.ToBase64String(image);
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

        public IEnumerable<BurialModel> ParceJson(string json)
        {
            var collection = JsonConvert.DeserializeObject<IEnumerable<ApiBurial>>(json);
            return collection.Select(x => new BurialModel(x));
        }

        private IBurialImageGuide mGuide;
    }
}