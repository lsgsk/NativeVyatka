using System.Threading.Tasks;
using Android.Content;
using System.IO;

namespace NativeVyatkaAndroid
{
    public class PhotoStorageManager
    {
        public PhotoStorageManager(Context context)
        {
            this.mContext = context;
        }

        public async Task<byte[]> LoadBurialImageFromFileSystemAsync(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = Path.GetFileName(name);
                if (IsFileExists(name))
                {
                    using (var fs = mContext.OpenFileInput(name))
                    {
                        if (fs.CanRead)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await fs.CopyToAsync(ms);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
            throw new FileNotFoundException("burial image not found: " + name);
        }

        public async Task SaveBurialImageToFileSystemAsync(string name, byte[] imageBytes)
        {
            if (imageBytes != null)
            {
                using (var fs = mContext.OpenFileOutput(name, FileCreationMode.Private))
                {
                    await fs.WriteAsync(imageBytes, 0, imageBytes.Length);
                }
            }
        }

        public bool IsFileExists(string filename)
        {    
            var file = mContext.GetFileStreamPath(filename);
            if (file == null || !file.Exists())
            {
                return false;
            }
            return true;
        }

        private readonly Context mContext;
    }
}

