using Abstractions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Controllers
{
    public class BurialEditController : BaseController, IBurialEditController
    {
        public BurialEditController(ICrossPageNavigator navigator,IBurialImageGuide burialImageGuide, IBurialStorage storage)
        {
            this.mNavigator = navigator;
            this.mBurialImageGuide = burialImageGuide;
            this.mStorage = storage;
        }

        private BurialModel burial;
        public BurialModel Burial
        {
            get
            {
                return burial;
            }
            set
            {
                burial = value;
                if(value.RecordTime == DateTime.MinValue)
                {
                    //это создание, а не редактирование
                    burial.RecordTime = DateTime.UtcNow;
                    Updated = true;
                }
            }
        }

        public void GoBackWithMeassage(string messgae)
        {
            mNavigator.GoToPage(PageStates.BulialListPage, new Dictionary<string, string>()
            {
                [FormBundleConstants.BackMessage] = messgae
            });         
        }

        public async Task<string> RetakePhotoAsync()
        {
           var newPhotoPath = await CreatePhoto();
            if(!string.IsNullOrEmpty(newPhotoPath))
            {
                await mBurialImageGuide.DeleteFromFileSystemAsync(Burial.PicturePath);
                Burial.PicturePath = newPhotoPath;
                Updated = true;
            }
            return Burial.PicturePath;
        }

        public async Task<DateTime?> SetBirthTimeAsync()
        {
            var time = await DatePromptAsync(Burial.BirthDay, maxTime: burial.DeathDay);
            if(time.Ok)
            {
                Burial.BirthDay = time.SelectedDate;
                Updated = true;
            }
            return Burial.BirthDay;
        }

        public async Task<DateTime?> SetDeathTimeAsync()
        {
            var time = await DatePromptAsync(Burial.DeathDay, minTime: Burial.BirthDay);
            if (time.Ok)
            {
                Burial.DeathDay = time.SelectedDate;
                Updated = true;
            }
            return Burial.DeathDay;
        }

        public async Task SaveAndUploadBurialAsync()
        {
            Progress = true;
            mStorage.InsertOrUpdateBurial(Burial);
            await Task.Delay(1000);
            //отправить,если успешно установть дату синхранизации
            Updated = false;
            Progress = false;            
        }

        public async Task SaveAndUploadBurialAndGoBackAsync()
        {
            if (Updated)
            {
                var confirm = await ConfirmAsync("Сохранить измения и сихранизировать запись?");
                if (confirm)
                {
                    await SaveAndUploadBurialAsync();
                }
            }
            mNavigator.GoToPage(PageStates.BulialListPage, new Dictionary<string, string>()
            {
                [FormBundleConstants.BackMessage] = "Запись обновлена"
            });
        }

        public async Task DeleteRecordAsync()
        {
            var confirm = await ConfirmAsync("Вы действительно хотите удалить запись?", "Внимание");
            if(confirm)
            {
                mStorage.DeleteBurial(Burial.CloudId);
            }
            mNavigator.GoToPage(PageStates.BulialListPage, new Dictionary<string, string>()
            {
                [FormBundleConstants.BackMessage] = "Запись удалена"
            });
        }      
        public event EventHandler<bool> BurialUpdated;
        private bool updated = false;
        public bool Updated
        {
            get
            {
                return updated;
            }
            set
            {
                updated = value;
                BurialUpdated?.Invoke(this, value);
            }
        }
        private readonly ICrossPageNavigator mNavigator;
        private readonly IBurialImageGuide mBurialImageGuide;
        private readonly IBurialStorage mStorage;
    }
}
