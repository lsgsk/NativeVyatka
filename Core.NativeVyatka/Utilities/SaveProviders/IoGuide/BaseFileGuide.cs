using Abstractions;
using Abstractions.Exceptions;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Utilities.SaveProviders.IoGuide
{
    public abstract class BaseFileGuide : IFileGuide
    {
        protected BaseFileGuide()
        {
            RootFolder = FileSystem.Current.LocalStorage;
        }

        protected virtual string FileNameConverting(string name)
        {
            return name;
        }

        public virtual async Task SaveToFileSystemAsync(byte[] fileBytes, string name)
        {
            fileBytes = fileBytes ?? new byte[0];
            name = FileNameConverting(Path.GetFileName(name));
            try
            {
                IFolder folder = await RootFolder.CreateFolderAsync(Subfolder, CreationCollisionOption.OpenIfExists);
                IFile image = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                using (var str = await image.OpenAsync(FileAccess.ReadAndWrite))
                {
                    await str.WriteAsync(fileBytes, 0, fileBytes.Length);
                }
            }
            catch(Exception ex)
            {
                iConsole.Error(ex);
                throw new FileGuideException();
            }
        }

        public virtual async Task<byte[]> LoadFromFileSystemAsync(string name)
        {
            name = FileNameConverting(name);
            try
            {
                IFolder folder = await RootFolder.CreateFolderAsync(Subfolder, CreationCollisionOption.OpenIfExists);
                IFile image = await folder.GetFileAsync(name);
                using (var stream = await image.OpenAsync(FileAccess.Read))
                {
                    using (MemoryStream memorystream = new MemoryStream())
                    {
                        stream.CopyTo(memorystream);
                        return memorystream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new FileGuideException();
            }
        }

        public string GetFullPath(string name)
        {
            return Path.Combine(RootFolder.Path, GetLocalPath(name));
        }
        public string GetLocalPath(string name)
        {
            return Path.Combine(Subfolder, FileNameConverting(Path.GetFileName(name)));
        }

        public virtual async Task<bool> CheckFileExistsAsync(string name)
        {
            return await RootFolder.CheckExistsAsync(GetLocalPath(name)) == ExistenceCheckResult.FileExists;
        }

        public virtual async Task DeleteFromFileSystemAsync(string name)
        {
            try
            {
                name = FileNameConverting(name);
                IFolder folder = await RootFolder.CreateFolderAsync(Subfolder, CreationCollisionOption.OpenIfExists);
                var image = await folder.GetFileAsync(name);
                await image.DeleteAsync();
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new FileGuideException();
            }
        }

        protected readonly IFolder RootFolder = FileSystem.Current.LocalStorage;
        protected abstract string Subfolder { get; }
    }
}
