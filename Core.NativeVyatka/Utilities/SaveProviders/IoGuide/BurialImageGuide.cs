using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Utilities.SaveProviders.IoGuide
{
    public class BurialImageGuide : BaseFileGuide, IBurialImageGuide
    {
        protected override string Subfolder
        {
            get
            {
                return "BurialFolder";
            }
        }
    }
}
