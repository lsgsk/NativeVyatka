using Abstractions.Models.AppModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Practices.Unity;
using Abstractions;
using Abstractions.Interfaces.Database.Tables;
using FluentAssertions;

namespace UnitTestProject.Database.Table
{
    [TestClass]
    public class Test_IProfileStorage
    {
        public static ProfileModel CreateProfile()
        {
            return new ProfileModel()
            {
                Uid = "10",
                Name = "Lysov Alexandr",
                Email = "qwe@qwe.ru",
                PictureUrl = @"https://upload.wikimedia.org/wikipedia/commons/thumb/7/76/MAKS2015part1-10_%28cropped%29.jpg/300px-MAKS2015part1-10_%28cropped%29.jpg",
            };
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        [TestMethod]
        public void CheckEmptyTable()
        {
            var container = TestInitialization.Container;
            var storage = container.Resolve<IProfileStorage>();
            var dbProfile = storage.GetProfile();
            dbProfile.Should().BeSameAs(ProfileModel.Null);
        }

        [TestMethod]
        public void SavingAndRestoringProfile()
        {
            var container = TestInitialization.Container;
            var storage = container.Resolve<IProfileStorage>();
            var stProfile = CreateProfile();
            storage.SaveProfile(stProfile);
            var dbProfile = storage.GetProfile();
            stProfile.ShouldBeEquivalentTo(dbProfile);
        }

        [TestMethod]
        public void RewritingProfile()
        {
            var container = TestInitialization.Container;
            var storage = container.Resolve<IProfileStorage>();
            var stProfile1 = CreateProfile(); stProfile1.Name = "Saturn";
            var stProfile2 = CreateProfile(); stProfile2.Name = "Jupiter";
            storage.SaveProfile(stProfile1);
            storage.SaveProfile(stProfile2);
            var dbProfile = storage.GetProfile();
            stProfile2.ShouldBeEquivalentTo(dbProfile);
        }

        [TestMethod]
        public void ClearProfile()
        {
            var container = TestInitialization.Container;
            var storage = container.Resolve<IProfileStorage>();
            var stProfile = CreateProfile();
            storage.SaveProfile(stProfile);
            storage.ClearProfile();
            var dbProfile = storage.GetProfile();
            dbProfile.Should().BeSameAs(ProfileModel.Null);
        }
    }
}
