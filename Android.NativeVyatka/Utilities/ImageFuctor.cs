using System;
using Android.Graphics;
using Android.Content;
using Java.Interop;
using Android.Views;
using System.Threading.Tasks;
using Abstractions;
using PCLStorage;
using NativeVyatkaAndroid;

namespace NativeVyatkaAndroid
{ 
    public class ImageFactor : IImageFactor
    {
        public ImageFactor(Context context)
        {
            this.mContext = context;
            context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.GetSize(mScreenSize);
            mScreenSize.X = (int)(mScreenSize.X * 0.7);
            mScreenSize.Y = (int)(mScreenSize.Y * 0.7);
        }

        public async Task SaveImageToFileSystemAsync(byte[] imageBytes, string name)
        {
            name = System.IO.Path.GetFileName(name);
            IFolder folder = await rootFolder.CreateFolderAsync(Subfolder, CreationCollisionOption.OpenIfExists);
            IFile image = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            var resizedBytes = await BitmapHelper.ResizeImageToScreen(imageBytes, mScreenSize.X, mScreenSize.Y);
            using (var str = await image.OpenAsync(FileAccess.ReadAndWrite))
            {
                await str.WriteAsync(resizedBytes, 0, resizedBytes.Length);    
            }             
        }

        public Task<byte[]> LoadImageFromFileSystemAsync(string name)
        {
            throw new NotSupportedException("It`s better to use Picasso to load images");
        }

        public string GetImagePath(string name)
        {
            name = System.IO.Path.GetFileName(name);
            return mContext.FilesDir.AbsolutePath + "/" + Subfolder + "/" + System.IO.Path.GetFileName(name);
        }

        public async Task<bool> CheckExistsAsync(string name)
        {
            name = System.IO.Path.GetFileName(name);
            return await rootFolder.CheckExistsAsync(GetImagePath(name)) == ExistenceCheckResult.FileExists;
        }

        private readonly Context mContext;
        private readonly Point mScreenSize = new Point();
        private readonly IFolder rootFolder = FileSystem.Current.LocalStorage;
        private const string Subfolder = "GreetingsImages";
    }
}

