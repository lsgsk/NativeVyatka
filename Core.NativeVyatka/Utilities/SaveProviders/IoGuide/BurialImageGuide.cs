using Abstractions;

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
