﻿using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface IBurialsNetworkProvider
    {
        Task UploadBurialsAsync(IEnumerable<BurialModel> burials);
    }
}
