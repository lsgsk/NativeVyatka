using Abstractions.Models.AppModels;
using System;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IBurialEditController: IBaseController
    {
        BurialModel Burial { get; set; }
        Task<string> RetakePhotoAsync();
        void SetBirthTime(string day, string month, string year);
        void SetDeathTime(string day, string month, string year);
        Task SaveAndUploadBurialAsync();
        Task SaveAndUploadBurialAndGoBackAsync();
        Task DeleteRecordAsync();
        bool Updated { get; set; }
        bool Creating { get;}
        event EventHandler<bool> BurialUpdated;
    }
}
