using System.Threading.Tasks;

namespace Abstractions
{
    public interface IFileGuide
    {
        Task SaveToFileSystemAsync(byte[] fileBytes, string name);
        Task<byte[]> LoadFromFileSystemAsync(string name);
        Task<bool> CheckFileExistsAsync(string name);
        Task DeleteFromFileSystemAsync(string name);
        string GetFullPath(string name);
        string GetLocalPath(string name);
    }

    public interface IBurialImageGuide : IFileGuide
    {

    }   
}

