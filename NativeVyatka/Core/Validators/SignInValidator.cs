using FluentValidation;
using System.Linq;
using Core.Properties;
using static NativeVyatka.SignInValidator;

namespace NativeVyatka
{
    public interface ISignInValidator
    {
        void VerifyEmailAndPassword(string email, string password);
    }

    public class SignInValidator : AbstractValidator<LoginAndPasswordPair>, ISignInValidator
    {
        public SignInValidator()
        {
            RuleFor(x => x.Email).NotNull().WithMessage(Resources.Validator_EmptyEmail);
            RuleFor(x => x.Email).NotEmpty().WithMessage(Resources.Validator_EmptyEmail);
            RuleFor(x => x.Password).NotNull().WithMessage(Resources.Validator_EmptyPassword);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Resources.Validator_EmptyPassword);
        }

        public void VerifyEmailAndPassword(string email, string password)
        {
            var res = this.Validate(new LoginAndPasswordPair() { Email = email, Password = password });
            if (!res.IsValid)
            {
                throw new NotValidLoginOrPasswordException()
                {
                    EmailMessage = res.Errors.FirstOrDefault(x => x.PropertyName == "Email")?.ErrorMessage,
                    PasswordMessage = res.Errors.FirstOrDefault(x => x.PropertyName == "Password")?.ErrorMessage,
                };
            }
        }

        public class LoginAndPasswordPair
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
