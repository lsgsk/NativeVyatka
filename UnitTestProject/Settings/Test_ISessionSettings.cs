using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Settings;
using System;
using FluentAssertions;
using Abstractions.Constants;

namespace UnitTestProject.Settings
{
    [TestClass]
    public class Test_ISessionSettings
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<ISessionSettings>().ClearPrefs();
        }

        [TestMethod]
        public void TestServiceUrlKey()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            const string url = "http://google.ru"; 
            settings.ServiceUrl.Should().Be(ApConstant.ServiceUrl);
            settings.ServiceUrl = url;
            settings.ServiceUrl.Should().Be(url);

        }
        [TestMethod]
        public void TestPushToken()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            var token = new Random().Next(int.MaxValue).ToString();
            settings.PushToken.Should().Be(string.Empty);
            settings.PushToken = token;
            settings.PushToken.Should().Be(token);
        }

        
        [TestMethod]
        public void TestCsrfToken()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            const string token = "qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq";
            settings.CsrfToken.Should().Be(string.Empty);
            settings.CsrfToken = token;
            settings.CsrfToken.Should().Be(token);
        }

        [TestMethod]
        public void TestSessionName()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            var name = DateTime.Now.ToLongTimeString(); ;
            settings.SessionName.Should().Be(string.Empty);
            settings.SessionName = name;
            settings.SessionName.Should().Be(name);
        }

        [TestMethod]
        public void TestSessionId()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            var id = new Guid().ToString();
            settings.SessionId.Should().Be(string.Empty);
            settings.SessionId = id;
            settings.SessionId.Should().Be(id);
        }

        [TestMethod]
        public void TestClearPrefs()
        {
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            settings.ServiceUrl = "qwe";
            settings.PushToken = "zxc";
            settings.CsrfToken = "asd";
            settings.SessionName = "sdf";
            settings.SessionId = "eet";
            settings.ClearPrefs();
            settings.ServiceUrl.Should().Be(ApConstant.ServiceUrl);
            settings.PushToken.Should().Be(string.Empty);
            settings.CsrfToken.Should().Be(string.Empty);
            settings.SessionName.Should().Be(string.Empty);
            settings.SessionId.Should().Be(string.Empty);
        }
    }
}
