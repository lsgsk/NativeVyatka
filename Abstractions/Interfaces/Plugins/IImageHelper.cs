using System;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IImageHelper
    {
        Task<byte[]> ToByteArrayAsync(string path);   
        byte[] ToByteArray(string path);
    }
}

