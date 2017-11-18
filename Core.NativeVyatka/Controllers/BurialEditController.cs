﻿using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Plugins;

using Abstractions.Models.AppModels;
using Acr.UserDialogs;
using NativeVyatkaCore.Properties;
using NativeVyatkaCore.Utilities;
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
                }
                if(!burial.Updated)
                {
                    //запись не синхранизирована, предлагаем сохранить
                    Updated = true;
                }
                if (!burial.Uploaded)
                {
                    //запись вообще не синхранизировалась, ее можно удалить
                    Removable = true;
                }
            }
        }

        public async Task<string> RetakePhotoAsync()
        {
            var newPhotoPath = await CreatePhoto();
            if (!string.IsNullOrEmpty(newPhotoPath))
            {
                //отменить смену фото невозможно
                await TryDeletePicture(Burial.PicturePath);
                Burial.PicturePath = newPhotoPath;
                Updated = true;
            }
            return Burial.PicturePath;
        }

        public void SetBirthTime(string day, string month, string year)
        {
            Burial.BirthDay = $"{(string.IsNullOrEmpty(day) ? "00" : day.ToString())}-" +
                              $"{(string.IsNullOrEmpty(month) ? "00" : month.ToString())}-" +
                              $"{(string.IsNullOrEmpty(year) ? "0000" : year.ToString())}";
        }

        public void SetDeathTime(string day, string month, string year)
        {
            Burial.DeathDay = $"{(string.IsNullOrEmpty(day) ? "00": day.ToString())}-" +
                              $"{(string.IsNullOrEmpty(month) ? "00" :  month.ToString())}-" +
                              $"{(string.IsNullOrEmpty(year) ? "0000" : year.ToString())}";
        }

        public async Task SaveAndUploadBurialAsync()
        {
            Progress = true;
            Burial.Updated = false;
            mStorage.InsertOrUpdateBurial(Burial);
            try
            {
                await mBurialsDataProvider.UploadBurialAsync(Burial);
                Updated = Progress = false;           
            }
            catch (BurialSyncException)
            {
                Progress = false;
                await AlertAsync(Resources.EditScreeen_SyncFailed, Resources.Dialog_Attention);
            }
            catch(Exception ex)
            {
                Progress = false;
                iConsole.Error(ex);
            }
            finally
            {
                mNavigator.Goback();
            }
        }

        public async Task SaveAndUploadBurialAndGoBackAsync()
        {
            if (Updated)
            {
                var confirm = await ConfirmAsync(Resources.EditScreeen_SaveAndSync, okText: Resources.Dialog_Yes, cancelText: Resources.Dialog_No);
                if (confirm)
                {
                    await SaveAndUploadBurialAsync();
                }
                else
                {
                    //если запись не сохранялась, картинку нужно удалить
                    if(mStorage.GetBurial(burial.CloudId) == BurialModel.Null)
                    {
                        await TryDeletePicture(Burial.PicturePath);
                    }
                    mNavigator.Goback();
                }
            }
            else
            {
                mNavigator.Goback();
            }
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
            mNavigator.Goback();
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
        public bool Removable { get; private set; } = false;
        private readonly ICrossPageNavigator mNavigator;
        private readonly IBurialImageGuide mBurialImageGuide;
        private readonly IBurialStorage mStorage;
        private readonly IBurialsNetworkProvider mBurialsDataProvider;
    }
}
