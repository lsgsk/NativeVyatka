using Abstractions.Interfaces.Database.Tables;
using System;
using Abstractions.Models.AppModels;
using Abstractions.Models.DatabaseModels;
using Abstractions.Exceptions;
using NativeVyatkaCore.Utilities;

namespace NativeVyatkaCore.Database
{
    public sealed partial class BurialDatabase : IProfileStorage
    {
        public ProfileModel GetProfile()
        {
            ProfileModel profile = null;
            using (var conn = GetConnection())
            {
                try
                {
                    var entity = conn.Table<ProfileEntity>().FirstOrDefault();
                    if (entity != null)
                    {
                        profile = new ProfileModel(entity);
                    }
                }
                catch (Exception ex)
                {
                    iConsole.Error(ex);
                }
            }
            return profile ?? ProfileModel.Null;
        }

        public void SavaProfile(ProfileModel profile)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    var entity = conn.Table<ProfileEntity>().FirstOrDefault();
                    if (entity == null)
                    {
                        conn.Insert(profile.ToProfileEntity());
                    }
                    else
                    {
                        var newprofile = profile.ToProfileEntity();
                        newprofile.Id = entity.Id;
                        conn.Update(newprofile);
                    }
                }
                catch (Exception ex)
                {
                    iConsole.Error(ex);
                    throw new ProfileSaveException();
                }
            }
        }
        public void ClearProfile()
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.DeleteAll<ProfileEntity>();
                }
                catch (Exception ex)
                {
                    iConsole.Error(ex);
                }
            }
        }
    }
}
