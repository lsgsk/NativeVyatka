namespace NativeVyatka
{
    public interface IBurialImageGuide : IFileGuide
    {
    }

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
