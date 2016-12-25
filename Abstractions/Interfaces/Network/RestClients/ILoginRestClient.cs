﻿using Abstractions.Models.Network.ServiceEntities;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface ILoginRestClient : IRestClient
    {
        Task<ApiProfile> LoginAsync(string login, string password, string pushToken);
        Task SiginAsync(string pushToken);
    }
}