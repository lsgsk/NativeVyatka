using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Plugins;
using Acr.UserDialogs;
using System;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Controllers
{
    public abstract class BaseController : IBaseController
    {
        public BaseController(IUserDialog dialogs)
        {
            this.mDialogs = dialogs;
        }

        public virtual void Dispose()
        {
        }
                
        public virtual bool Progress
        {
            set
            {
                mDialogs.Progress = value;                
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
            return mDialogs.DatePromptAsync(time, maxTime, minTime);
        }

        protected Task<string> CreatePhoto()
        {
            return mDialogs.CreatePhoto();            
        }

        private readonly IUserDialog mDialogs;
    }
}
