using System;
using Abstractions;
using System.Threading.Tasks;
using Java.IO;
using Android.Graphics;
using System.IO;

namespace Plugins
{
    public class ImageHelperRealization : IImageHelper
    {
        public ImageHelperRealization()
        {
        }

        public async Task<byte[]> ToByteArrayAsync(string path)
        {
            try
            {
                var file = new Java.IO.File(path);
                byte[] data = new byte[(int)file.Length()];
                await new FileInputStream(file).ReadAsync(data);
                return data;
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
            return new byte[0];
        }

        public byte[] ToByteArray(string path)
        {    
            try
            {
                var file = new Java.IO.File(path);
                byte[] data = new byte[(int)file.Length()];
                new FileInputStream(file).Read(data);
                return data;
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
            return new byte[0];
        }
    }
}

