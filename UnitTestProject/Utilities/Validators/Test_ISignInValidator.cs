using Microsoft.VisualStudio.TestTools.UnitTesting;
using NativeVyatkaCore.Utilities.Validators;
using Microsoft.Practices.Unity;
using Abstractions.Exceptions;
using FluentAssertions;
using Abstractions.Interfaces.Utilities.Validators;

namespace UnitTestProject.Utilities.Validators
{
    [TestClass]
    public class Test_ISignInValidator
    {
        [TestMethod]
        public void ValidLogin()
        {
            var validator = TestInitialization.Container.Resolve<ISignInValidator>();
            validator.VerifyEmailAndPassword("qwe@qwe.ru", "qweq");
        }

        [TestMethod]
        public void NullLoginAndPassword()
        {
            try
            {
                var validator = TestInitialization.Container.Resolve<ISignInValidator>();
                validator.VerifyEmailAndPassword(null, null);
                Assert.Fail();
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                ex.EmailMessage.Should().Be(SignInValidator.EmptyEmail);
                ex.PasswordMessage.Should().Be(SignInValidator.EmptyPassword);
            }
        }

        [TestMethod]
        public void NullPassword()
        {
            try
            {
                var validator = TestInitialization.Container.Resolve<ISignInValidator>();
                validator.VerifyEmailAndPassword("qwe@qwe.ru", null);
                Assert.Fail();
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                ex.PasswordMessage.Should().Be(SignInValidator.EmptyPassword);
            }
        }

        [TestMethod]
        public void EmptyLogin()
        {
            try
            {
                var validator = TestInitialization.Container.Resolve<ISignInValidator>();
                validator.VerifyEmailAndPassword(string.Empty, string.Empty);
                Assert.Fail();
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                ex.EmailMessage.Should().Be(SignInValidator.EmptyEmail);
                ex.PasswordMessage.Should().Be(SignInValidator.EmptyPassword);
            }
        }       

    }
}
