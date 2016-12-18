using Abstractions.Models.AppModels;
using System;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IBurialEditController: IBaseController
    {
        BurialModel Burial { get; set; }
        void ForceGoBack();
        Task<string> RetakePhotoAsync();
        Task<DateTime?> SetBirthTimeAsync();
        Task<DateTime?> SetDeathTimeAsync();
        Task SaveAndUploadBurialAsync();
        Task SaveAndUploadBurialAndGoBackAsync();
        Task DeleteRecordAsync();
        bool Updated { get; set; }
        bool Creating { get;}
        event EventHandler<bool> BurialUpdated;
    }
}
