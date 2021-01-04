using System;

namespace NativeVyatka
{
    public interface IProfileStorage
    {
        void SaveProfile(ProfileModel profile);
        ProfileModel GetProfile();
        void ClearProfile();
    }

    public sealed partial class BurialDatabase : IProfileStorage
    {
        public ProfileModel GetProfile() {
            try {
                using var conn = GetConnection();
                var entity = conn.Table<ProfileEntity>().FirstOrDefault();
                if (entity != null) {
                    return new ProfileModel(entity);
                }
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
            throw new ProfileException();
        }

        public void SaveProfile(ProfileModel profile) {
            try {
                using var conn = GetConnection();
                var entity = conn.Table<ProfileEntity>().FirstOrDefault();
                if (entity == null) {
                    conn.Insert(profile.ToProfileEntity());
                }
                else {
                    var newprofile = profile.ToProfileEntity();
                    newprofile.Id = entity.Id;
                    conn.Update(newprofile);
                }
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
        }

        public void ClearProfile() {
            try {
                using var conn = GetConnection();
                conn.DeleteAll<ProfileEntity>();
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
        }
    }
}
