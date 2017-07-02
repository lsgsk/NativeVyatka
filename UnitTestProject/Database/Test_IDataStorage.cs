using Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Database.Tables;
using UnitTestProject.Database.Table;
using FluentAssertions;
using Abstractions.Models.AppModels;

namespace UnitTestProject.Database
{
    [TestClass]
    public class Test_IDataStorage
    {
        [TestMethod]
        public void ValidateCleaningTable()
        {
            var storage = Test.Container.Resolve<IDataStorage>();
            var bstorage = Test.Container.Resolve<IBurialStorage>();
            var pstorage = Test.Container.Resolve<IProfileStorage>();
            foreach(var item in Test_IBurialStorage.CreateBurialsCollection())
            {
                bstorage.InsertOrUpdateBurial(item);
            }           
            pstorage.SaveProfile(Test_IProfileStorage.CreateProfile());
            storage.ClearDataBase();
            bstorage.GetBurials().Should().BeEmpty();
            pstorage.GetProfile().Should().Equals(ProfileModel.Null);
        }
    }
}
