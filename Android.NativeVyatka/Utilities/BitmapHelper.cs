using System;
using Android.Graphics;
using System.Threading.Tasks;
using System.IO;

namespace NativeVyatkaAndroid
{
    public static class BitmapHelper
    {
        public async static Task<byte[]> ResizeImageToScreen(byte[] input, int screenWidth, int screenHeight)
        {   
            using (var original = await BitmapFactory.DecodeByteArrayAsync(input, 0, input.Length))
            {
                int PHOTO_WIDTH = 0, PHOTO_HEIGHT = 0;
                if (original.Width > screenWidth || original.Height > screenHeight)
                {
                    var max = Math.Max(original.Width, original.Height);
                    if (max == original.Height && original.Height > screenHeight)
                    {
                        PHOTO_HEIGHT = screenHeight;
                        PHOTO_WIDTH = (int)(original.Width * (screenHeight / (double)original.Height));
                    }
                    else
                    {
                        PHOTO_WIDTH = screenWidth;
                        PHOTO_HEIGHT = (int)(original.Height * (screenWidth / (double)original.Width));
                    }                   
                }
                if (PHOTO_HEIGHT * PHOTO_WIDTH != 0)
                {
                    using (var resized = Bitmap.CreateScaledBitmap(original, PHOTO_WIDTH, PHOTO_HEIGHT, true))
                    {
                        var blob = new MemoryStream();
                        await resized.CompressAsync(Bitmap.CompressFormat.Png, 70, blob);
                        return blob.ToArray();
                    }
                }
                else
                {
                    return input;
                }
            }
        }

        public async static Task<byte[]> ToByteArray(Bitmap b)
        {
            using (var stream = new MemoryStream())
            {
                await b.CompressAsync(Bitmap.CompressFormat.Png, 70, stream);
                return stream.ToArray();
            }
        }
    }
}
            

