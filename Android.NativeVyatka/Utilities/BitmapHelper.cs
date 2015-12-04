using System;
using Android.Graphics;
using System.Threading.Tasks;
using System.IO;

namespace NativeVyatkaAndroid
{
    public static class BitmapHelper
    {
        private const int baseImageSize = 700;

        public static int CalculateInSampleSize(BitmapFactory.Options options)
        {
            return CalculateInSampleSize(options, baseImageSize, baseImageSize);
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = options.OutHeight;
            var width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                var halfHeight = height / 2;
                var halfWidth = width / 2;
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }
            return inSampleSize;
        }

        public async static Task<byte[]> ResizeImage(byte[] input)
        {
            return await ResizeImage(input, baseImageSize, baseImageSize);
        }

        public async static Task<byte[]> ResizeImage(byte[] input, int maxWidth, int maxHeight) 
        {     
            using (var original = await BitmapFactory.DecodeByteArrayAsync(input, 0, input.Length))
            {
                int PHOTO_WIDTH = 0, PHOTO_HEIGHT = 0;
                if (original.Width > maxWidth || original.Height > maxHeight)
                {
                    var max = Math.Max(original.Width, original.Height);
                    if (max == original.Height && original.Height > maxHeight)
                    {
                        PHOTO_HEIGHT = maxWidth;
                        PHOTO_WIDTH = (int)(original.Width * (maxHeight/(double)original.Height));
                    }
                    else
                    {
                        PHOTO_WIDTH = maxWidth;
                        PHOTO_HEIGHT = (int)(original.Height * (maxWidth/(double)original.Width));
                    }                   
                }
                if (PHOTO_HEIGHT * PHOTO_WIDTH != 0)
                {
                    using (var resized = Bitmap.CreateScaledBitmap(original, PHOTO_WIDTH, PHOTO_HEIGHT, true))
                    {
                        var blob = new MemoryStream();
                        await resized.CompressAsync(Bitmap.CompressFormat.Png, 100, blob);
                        return blob.ToArray();
                    }
                }
                else
                {
                    return input;
                }
            }
        }

        public async static Task<byte[]> ResizeImage(Bitmap original)
        {
            return await ResizeImage(original, baseImageSize, baseImageSize);
        }

        public async static Task<byte[]> ResizeImage(Bitmap original, int maxWidth, int maxHeight)
        {
            int PHOTO_WIDTH = 0, PHOTO_HEIGHT = 0;
            if (original.Width > maxWidth || original.Height > maxHeight)
            {
                var max = Math.Max(original.Width, original.Height);
                if (max == original.Height && original.Height > maxHeight)
                {
                    PHOTO_HEIGHT = maxWidth;
                    PHOTO_WIDTH = (int)(original.Width * (maxHeight/(double)original.Height));
                }
                else
                {
                    PHOTO_WIDTH = maxWidth;
                    PHOTO_HEIGHT = (int)(original.Height * (maxWidth/(double)original.Width));
                }                   
            }
            if (PHOTO_HEIGHT * PHOTO_WIDTH != 0)
            {
                using (var resized = Bitmap.CreateScaledBitmap(original, PHOTO_WIDTH, PHOTO_HEIGHT, true))
                {
                    var blob = new MemoryStream();
                    await resized.CompressAsync(Bitmap.CompressFormat.Png, 100, blob);
                    return blob.ToArray();
                }
            }
            else
            {
                return await ToByteArray(original);
            }
        }

        public static Task<Bitmap> ToBitmap (byte[] imagedata)
        {
            return BitmapFactory.DecodeByteArrayAsync (imagedata, 0, imagedata.Length);
        }

        public static async Task<byte[]> ToByteArray(Bitmap b)
        {
            byte[] res;
            using (var stream = new MemoryStream ()) 
            {
                await b.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
                byte[] byteArray = stream.ToArray();
                res = byteArray;
            }
            return res;
        }
    }
}

