using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Settings;
using Abstractions;
using Abstractions.Models.AppModels;
using Abstractions.Interfaces.Network;
using System.Threading;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Exceptions;
using Moq;
using UnitTestProject.Database.Table;

namespace UnitTestProject.Network
{
    [TestClass]
    public class Test_IBurialsNetworkProvider
    {
        public static IEnumerable<BurialModel> CreateBurialCollection()
        {
            return Test_IBurialStorage.CreateBurialsCollection();
        }

        public static IBurialsNetworkProvider CreateProvider(bool networkSuccess = true, CancellationToken token = new CancellationToken())
        {
            var mock = new Mock<IBurialRestClient>();
            mock.Setup(x => x.UploadBurialAsync(It.IsAny<IEnumerable<BurialModel>>())).Returns(async () =>
            {
                await Task.Delay(0);
                if (!networkSuccess || token.IsCancellationRequested)
                {                  
                    throw new BurialUploadException();
                }
            });          
            var container = Test.CreateChildContainer();
            container.RegisterInstance(mock.Object);
            return container.Resolve<IBurialsNetworkProvider>();
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            Test.Container.Resolve<ISettingsProvider>().ClearPrefs();
            Test.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        [TestMethod]
        public async Task SuccessUploading()
        {
            var provider = CreateProvider();
            await provider.UploadBurialsAsync(CreateBurialCollection());
        }

        [TestMethod]
        public async Task SuccessWithNullCollection()
        {
            var provider = CreateProvider();
            await provider.UploadBurialsAsync(null);
        }

        [TestMethod]
        public async Task FailsUploading()
        {
            try
            {
                var provider = CreateProvider(false);
                await provider.UploadBurialsAsync(CreateBurialCollection());
            }
            catch(BurialSyncException)
            {
            }
        }
        [TestMethod]
        public async Task CancelUploading()
        {
            try
            {
                var provider = CreateProvider(true, new CancellationToken(true));
                await provider.UploadBurialsAsync(CreateBurialCollection());
            }
            catch (BurialSyncException)
            {
            }
        }
    }
}