using System;
using Abstractions.Interfaces.Plugins;
using Acr.UserDialogs;
using System.Threading.Tasks;
using Plugin.Media;
using System.IO;

namespace NativeVyatkaAndroid.Utilities
{
    public class UserDialogRealization : IUserDialog
    {
        public bool Progress
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

        public Task AlertAsync(string message, string title = null)
        {
            return UserDialogs.Instance.AlertAsync(message, title);
        }
        public Task<bool> ConfirmAsync(string message, string title = null)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title);
        }
        public Task<DatePromptResult> DatePromptAsync(DateTime? time = null, DateTime? maxTime = null, DateTime? minTime = null)
        {
            return UserDialogs.Instance.DatePromptAsync(new DatePromptConfig()
            {
                SelectedDate = time ?? DateTime.UtcNow,
                UnspecifiedDateTimeKindReplacement = DateTimeKind.Utc,
                MaximumDate = maxTime,
                MinimumDate = minTime
            });
        }

        public async Task<string> CreatePhoto()
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