using System.Collections.Generic;
using ServiceStack;

namespace Abstractions
{
    [Route("/burials", "POST")]
    public class ApiBurialEntityCollectionRequest : IReturn<bool>
    {
        public IEnumerable<ApiBurialEntity> Items { get; set;}
    }
}

