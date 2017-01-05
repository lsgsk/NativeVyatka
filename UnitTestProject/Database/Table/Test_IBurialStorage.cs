using Abstractions;
using Abstractions.Models.AppModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using System.Threading;
using Abstractions.Interfaces.Database.Tables;
using FluentAssertions;
using System.Linq;

namespace UnitTestProject.Database.Table
{
    [TestClass]
    public class Test_IBurialStorage
    {
        public static BurialModel CreateBurial()
        {
            var rd = new Random((int)DateTime.Now.Ticks);
            return new BurialModel()
            {
                Name = rd.Next(int.MaxValue).ToString(),
                Surname = rd.Next(int.MaxValue).ToString(),
                Patronymic = rd.Next(int.MaxValue).ToString(),
                Description = rd.Next(int.MaxValue).ToString(),
                BirthDay = DateTime.UtcNow,
                DeathDay = DateTime.UtcNow,
                PicturePath = "folder/image.png",
                Updated = (rd.Next(100) %2 == 0),
                Location = new BurialModel.Position()
                {
                    Altitude = rd.NextDouble(),
                    Heading = rd.NextDouble(),
                    Latitude = rd.NextDouble(),
                    Longitude = rd.NextDouble()
                }
            };
        }

        public static List<BurialModel> CreateBurialsCollection()
        {
            var list = new List<BurialModel>();
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(50);
                list.Add(CreateBurial());
            }
            return list;
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<IDataStorage>().ClearDataBase();
        }     
        
        [TestMethod]
        public void CheckEmptyBurialsTable()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var dbCollection = storage.GetBurials();
            dbCollection.Should().NotBeNull();
            dbCollection.Should().HaveCount(0);
        }

        [TestMethod]
        public void SaveAndRestoreBurials()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var collection = CreateBurialsCollection();
            foreach (var burial in collection)
                storage.InsertOrUpdateBurial(burial);
            var dbCollection = storage.GetBurials();
            dbCollection.ShouldAllBeEquivalentTo(collection);
        }

        [TestMethod]
        public void GetNotSyncBurials()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var collection = CreateBurialsCollection();
            foreach (var burial in collection)
                storage.InsertOrUpdateBurial(burial);
            var dbCollection = storage.GetNotSyncBurials();
            dbCollection.ShouldAllBeEquivalentTo(collection.Where(x => !x.Updated).ToList());
        }

        [TestMethod]
        public void SaveAndRestoreBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(burial);
        }       

        [TestMethod]
        public void CheckFindIncorrextBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId + 1);
            dbBurial.Should().Be(BurialModel.Null);
        }

        [TestMethod]
        public void CheckTimeInUtcBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            burial.RecordTime = DateTime.UtcNow;
            burial.BirthDay = DateTime.UtcNow;
            burial.DeathDay = DateTime.UtcNow;
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.RecordTime.Should().Be(burial.RecordTime);
            dbBurial.RecordTime.Kind.Should().Be(DateTimeKind.Utc);
            dbBurial.BirthDay.Value.Should().Be(burial.BirthDay.Value);
            dbBurial.BirthDay.Value.Kind.Should().Be(DateTimeKind.Utc);
            dbBurial.DeathDay.Value.Should().Be(burial.DeathDay.Value);
            dbBurial.DeathDay.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        [TestMethod]
        public void SaveUpdateAndRestoreBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(burial);
            burial.Name = "qqqqqqqqqqqqqqqqqqqq";
            storage.InsertOrUpdateBurial(burial);
            dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(burial);
        }

        [TestMethod]
        public void DeleteBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(burial);
            storage.DeleteBurial(burial.CloudId);
            dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.Should().Be(BurialModel.Null);
        }      
        
        [TestMethod]
        public void TwiceInsertedBurial()
        {
            var storage = TestInitialization.Container.Resolve<IBurialStorage>();
            var burial = CreateBurial();
            storage.InsertOrUpdateBurial(burial);
            storage.InsertOrUpdateBurial(burial);
            var dbBurial = storage.GetBurial(burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(burial);//а так ли нужно
            storage.GetBurials().Should().HaveCount(1);
        }
    }
}
