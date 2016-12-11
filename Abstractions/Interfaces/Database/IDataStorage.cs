using Abstractions.Interfaces.Database.Tables;

namespace Abstractions
{
    public interface IDatabase : IBurialStorage, IProfileStorage
    {
        void ClearDataBase();        
    }
}

