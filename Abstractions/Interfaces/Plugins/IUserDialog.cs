using Acr.UserDialogs;
using System;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Plugins
{
    public interface IUserDialog
    {
        bool Progress { set; }
        Task AlertAsync(string message, string title = null);
        Task<bool> ConfirmAsync(string message, string title = null);
        Task<DatePromptResult> DatePromptAsync(DateTime? time = null, DateTime? maxTime = null, DateTime? minTime = null);
        Task<string> CreatePhoto();
    }
}
