﻿using Abstractions.Interfaces.Controllers;
using Acr.UserDialogs;
using NativeVyatkaCore.Properties;
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

        public virtual Task<bool> ConfirmAsync(string message, string title = null)
        {
            return mDialogs.ConfirmAsync(message, title);
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
            if (mMedia.IsCameraAvailable && mMedia.IsTakePhotoSupported)
            {
                var file = await mMedia.TakePhotoAsync(new StoreCameraMediaOptions
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
                await AlertAsync(Resources.MainScreeen_CameraNotAvailable, Resources.Dialog_Attention);
            }
            return string.Empty;
        }

        private readonly IUserDialogs mDialogs;
        private readonly IMedia mMedia;
    }
}
