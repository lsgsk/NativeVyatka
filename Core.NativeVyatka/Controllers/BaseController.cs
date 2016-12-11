using Abstractions.Interfaces.Controllers;
using Acr.UserDialogs;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Controllers
{
    public class BaseController : IBaseController
    {
        public virtual void Dispose()
        {

        }
                
        public virtual bool Progress
        {
            set
            {
                if (value)
                {
                    UserDialogs.Instance.ShowLoading();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        }

        public virtual Task AlertAsync(string message, string title = null)
        {
            return UserDialogs.Instance.AlertAsync(message, title);
        }

        public virtual Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        }

        public virtual Task<DatePromptResult> DatePromptAsync(DateTime? time = null, DateTime? maxTime = null, DateTime? minTime = null)
        {
            return UserDialogs.Instance.DatePromptAsync(new DatePromptConfig()
            {
                SelectedDate = time ?? DateTime.UtcNow,
                UnspecifiedDateTimeKindReplacement = DateTimeKind.Utc,
                MaximumDate = maxTime,
                MinimumDate = minTime
            });
        }

        protected async Task<string> CreatePhoto()
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    CustomPhotoSize = 50,
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
                await AlertAsync("No Camera", ":( No camera available.");
            }
            return string.Empty;
        }
    }
}
