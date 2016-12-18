﻿using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface IBurialsDataProvider
    {
        Task DownloadBurialsAsync();
        Task UploadBurial(BurialModel burial);
    }
}
