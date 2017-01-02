using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Abstractions;
using System.IO;
using FluentAssertions;
using Abstractions.Exceptions;

namespace UnitTestProject.SaveProviders.IoGuide
{
    [TestClass]
    public class Test_BurialImageGuide
    {
        private const string Name = "picture.png";

        [TestInitialize]
        public void PrepareDatabase()
        {
            var path = TestInitialization.Container.Resolve<IBurialImageGuide>().GetFullPath(string.Empty);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        [TestMethod]
        public async Task SaveFileAndLoad()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            var image = new byte[] { 0, 1, 2, 3, 3, 5 };
            await guide.SaveToFileSystemAsync(image, Name);
            var file = await guide.LoadFromFileSystemAsync(Name);
            image.ShouldBeEquivalentTo(file);
        }

        [TestMethod]
        public async Task SaveNullFileAndLoad()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            byte[] image = null;
            await guide.SaveToFileSystemAsync(image, Name);
            var file = await guide.LoadFromFileSystemAsync(Name);
            new byte[0].ShouldBeEquivalentTo(file);
        }

        [TestMethod]
        public async Task SaveEmptyFileAndLoad()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            byte[] image = new byte[0];
            await guide.SaveToFileSystemAsync(image, Name);
            var file = await guide.LoadFromFileSystemAsync(Name);
            image.ShouldBeEquivalentTo(file);
        }

        [TestMethod]
        public async Task LoadNotExistingFile()
        {
            try
            {
                var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
                var file = await guide.LoadFromFileSystemAsync(Name);
                Assert.Fail();
            }
            catch (FileGuideException)
            {
            }
        }

        [TestMethod]
        public async Task CheckExistingFile()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            byte[] image = new byte[] { 0, 1, 2, 3, 3, 5 };
            await guide.SaveToFileSystemAsync(image, Name);
            (await guide.CheckFileExistsAsync(Name)).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckNotExistingFile()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            (await guide.CheckFileExistsAsync(Name)).Should().BeFalse();
        }

        [TestMethod]
        public async Task DeleteExistingFile()
        {
            var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
            var image = new byte[] { 0, 1, 2, 3, 3, 5 };
            await guide.SaveToFileSystemAsync(image, Name);
            await guide.DeleteFromFileSystemAsync(Name);
        }

        [TestMethod]
        public async Task DeleteNotExistingFile()
        {
            try
            {
                var guide = TestInitialization.Container.Resolve<IBurialImageGuide>();
                await guide.DeleteFromFileSystemAsync(Name);
                Assert.Fail();
            }
            catch(FileGuideException)
            {
            }
        }
    }
}
