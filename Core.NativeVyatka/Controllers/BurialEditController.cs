using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Abstractions.Models.AppModels;
using Acr.UserDialogs;
using NativeVyatkaCore.Properties;
using Plugin.Media.Abstractions;
using System;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Controllers
{
    public class BurialEditController : BaseController, IBurialEditController
    {
        public BurialEditController(ICrossPageNavigator navigator, IBurialImageGuide burialImageGuide, IBurialStorage storage, IBurialsNetworkProvider burialsDataProvider, IUserDialogs dialogs, IMedia media) : base(dialogs, media)
        {
            this.mNavigator = navigator;
            this.mBurialImageGuide = burialImageGuide;
            this.mStorage = storage;
            this.mBurialsDataProvider = burialsDataProvider;
        }

        private BurialModel burial;
        public BurialModel Burial
        {
            get
            {
                return burial ?? BurialModel.Null;
            }
            set
            {
                burial = value;
                if(value.RecordTime == DateTime.MinValue)
                {
                    //это создание, а не редактирование
                    burial.RecordTime = DateTime.UtcNow;
                    Updated = true;
                    Creating = true;
                }
                if(!burial.Updated)
                {
                    //запись не синхранизирована, предлагаем сохранить
                    Updated = true;
                }
            }
        }

        public async Task<string> RetakePhotoAsync()
        {
            var newPhotoPath = await CreatePhoto();
            if (!string.IsNullOrEmpty(newPhotoPath))
            {
                await TryDeletePicture(Burial.PicturePath);
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
            Burial.Updated = false;
            mStorage.InsertOrUpdateBurial(Burial);
            try
            {
                await mBurialsDataProvider.UploadBurialsAsync(new[] { Burial });
                Updated = Progress = false;
                await AlertAsync(Resources.EditScreen_SyncSuccess);
                mNavigator.Goback();
            }
            catch (BurialSyncException)
            {
                Progress = false;
                await AlertAsync(Resources.EditScreeen_SyncFailed, Resources.Dialog_Attention);
            }
        }

        public async Task SaveAndUploadBurialAndGoBackAsync()
        {
            if (Updated)
            {
                var confirm = await ConfirmAsync(Resources.EditScreeen_SaveAndSync);
                if (confirm)
                {
                    await SaveAndUploadBurialAsync();                    
                }
            }
            mNavigator.GoToPage(PageStates.BulialListPage);
        }

        public async Task DeleteRecordAsync()
        {
            var confirm = await ConfirmAsync(Resources.EditScreeen_DeleteQuestion, Resources.Dialog_Attention);
            if (confirm)
            {
                await TryDeletePicture(Burial.PicturePath);
                mStorage.DeleteBurial(Burial.CloudId);
                await AlertAsync(Resources.EditScreeen_DeleteFinised);
            }
            mNavigator.GoToPage(PageStates.BulialListPage);
        }  
        
        private async Task TryDeletePicture(string picturePath)
        {
            try
            {
                await mBurialImageGuide.DeleteFromFileSystemAsync(picturePath);
            }
            catch(FileGuideException)
            {

            }
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
        public bool Creating { get; private set; } = false;
        private readonly ICrossPageNavigator mNavigator;
        private readonly IBurialImageGuide mBurialImageGuide;
        private readonly IBurialStorage mStorage;
        private readonly IBurialsNetworkProvider mBurialsDataProvider;
    }
}
