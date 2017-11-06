using Abstractions.Interfaces.Controllers;
using Acr.UserDialogs;
using NativeVyatkaCore.Properties;
using NativeVyatkaCore.Utilities;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Controllers
{
    public abstract class BaseController : IBaseController
    {
        public BaseController(IUserDialogs dialogs, IMedia media)
        {
            this.mDialogs = dialogs;
            this.mMedia = media;
        }

        public virtual void Dispose()
        {
        }
                
        public virtual bool Progress
        {
            set
            {
                if (value)
                {
                    mDialogs.ShowLoading();
                }
                else
                {
                    mDialogs.HideLoading();
                }
            }
        }

        public virtual Task AlertAsync(string message, string title = null)
        {
            return mDialogs.AlertAsync(message, title);
        }

        public virtual Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
        {
            return mDialogs.ConfirmAsync(message, title, okText, cancelText);
        }

        public virtual Task<DatePromptResult> DatePromptAsync(DateTime? time = null, DateTime? maxTime = null, DateTime? minTime = null)
        {
            return mDialogs.DatePromptAsync(new DatePromptConfig()
            {
                SelectedDate = time ?? DateTime.UtcNow,
                UnspecifiedDateTimeKindReplacement = DateTimeKind.Utc,
                MaximumDate = maxTime,
                MinimumDate = minTime
            });
        }

        protected async Task<string> CreatePhoto()
        {
            try
            {
                if (mMedia.IsCameraAvailable && mMedia.IsTakePhotoSupported)
                {
                    var file = await mMedia.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Small,
                        CompressionQuality = 80,
                        Directory = "BurialFolder",
                        Name = Path.GetRandomFileName() + ".jpg"
                    });
                    if (file != null)
                    {
                        return file.Path;
                    }
                }
                else
                {
                    await AlertAsync(Resources.MainScreeen_CameraNotAvailable, Resources.Dialog_Attention);
                }
            }
            catch(Exception ex)
            {
                iConsole.Error(ex);
                await AlertAsync(Resources.MainScreeen_CameraNotAvailable, Resources.Dialog_Attention);
            }           
            return string.Empty;
        }

        private readonly IUserDialogs mDialogs;
        private readonly IMedia mMedia;
    }
}
