using Abstractions.Models.AppModels;

namespace Abstractions.Interfaces.Database.Tables
{
    public interface IProfileStorage
    {
        void SavaProfile(ProfileModel profile);
        ProfileModel GetProfile();
        void ClearProfile();
    }
}
