using System.Threading.Tasks;

namespace iLobby.Portable
{
    public interface IImageFactor
    {
        Task SaveImageToFileSystemAsync(byte[] imageBytes, string name);      
        Task<byte[]> LoadImageFromFileSystemAsync(string name);
        Task<bool> CheckExistsAsync(string name);
        string GetImagePath(string name);
    }
}

