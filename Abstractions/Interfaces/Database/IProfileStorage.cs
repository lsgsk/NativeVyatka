using Abstractions.Models.AppModels;

namespace Abstractions.Interfaces.Database.Tables
{
    public interface IProfileStorage
    {
        void SaveProfile(ProfileModel profile);
        ProfileModel GetProfile();
        void ClearProfile();
    }
}
