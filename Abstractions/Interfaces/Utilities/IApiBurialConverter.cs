using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Utilities
{
    public interface IApiBurialConverter
    {
        Task<ApiBurialToSend> Convert(BurialModel model);
        Task<string> Serialize(BurialModel model);
        IEnumerable<BurialModel> ParceJson(string json);
    }
}
