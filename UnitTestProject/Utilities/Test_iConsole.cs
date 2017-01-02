using Abstractions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NativeVyatkaCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Utilities
{
    [TestClass]
    public class Test_iConsole
    {
        [TestMethod]
        public void TryWriteWithoutInitialization()
        {
            iConsole.WriteLine("test");
        }
        [TestMethod]
        public void TryWriteMessage()
        {
            string testmessage = "test";
            var mock = new Mock<IConsole>();
            mock.Setup(a => a.WriteLine(It.IsAny<string>())).Callback((string message) => testmessage.Should().Be(message)).Verifiable();
            iConsole.Init(mock.Object);
            iConsole.WriteLine(testmessage);
        }
        [TestMethod]
        public void TryWriteException()
        {
            var exception = new Exception();
            var mock = new Mock<IConsole>();
            mock.Setup(a => a.Error(It.IsAny<Exception>())).Callback((Exception ex) => ex.Should().Be(exception)).Verifiable();
            iConsole.Init(mock.Object);
            iConsole.Error(exception);
        }
    }
}
