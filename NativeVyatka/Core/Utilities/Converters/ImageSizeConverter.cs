using System.IO;
using Android.Graphics;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using System;

namespace NativeVyatka
{
    //хуета нужно разделять сохранение и конвертация
    public interface IImageSizeConverter
    {
        Task<string> ResizeImage(MediaFile file);
    }

    public class ImageSizeConverter: IImageSizeConverter
    {
        public async Task<string> ResizeImage(MediaFile file) {
            try {
                using var stream = file.GetStream();
                using var bitmap = BitmapFactory.DecodeStream(stream);

                int newWidth = (bitmap.Height > bitmap.Width) ? bitmap.Width : bitmap.Height;
                int newHeight = (bitmap.Height > bitmap.Width) ? bitmap.Height - (bitmap.Height - bitmap.Width) : bitmap.Height;
                int cropW = (bitmap.Width - bitmap.Height) / 2;
                cropW = (cropW < 0) ? 0 : cropW;
                int cropH = (bitmap.Height - bitmap.Width) / 2;
                cropH = (cropH < 0) ? 0 : cropH;
                Bitmap cropImg = Bitmap.CreateBitmap(bitmap, cropW, cropH, newWidth, newHeight);

                byte[] bitmapData;
                using var mstream = new MemoryStream();
                await cropImg.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, mstream);
                bitmapData = mstream.ToArray();
                File.Delete(file.Path);
                await File.WriteAllBytesAsync(file.Path, bitmapData);
                return file.Path;
            }
            catch(Exception ex) {
                iConsole.Error(ex);
                throw new ImageConversionException(ex.Message);
            }
        }
    }

    public class ImageConversionException: Exception
    {
        public ImageConversionException(string message): base(message) {
        }
    }
}
