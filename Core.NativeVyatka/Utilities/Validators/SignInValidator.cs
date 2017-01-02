using Abstractions.Exceptions;
using Abstractions.Interfaces.Utilities.Validators;
using FluentValidation;
using static NativeVyatkaCore.Utilities.Validators.SignInValidator;
using System.Linq;

namespace NativeVyatkaCore.Utilities.Validators
{
    public class SignInValidator : AbstractValidator<LoginAndPasswordPair>, ISignInValidator
    {
        public SignInValidator()
        {
            RuleFor(x => x.Email).NotNull().WithMessage(EmptyEmail);
            RuleFor(x => x.Email).NotEmpty().WithMessage(EmptyEmail);
            When(x => !string.IsNullOrEmpty(x.Email), () => RuleFor(x => x.Email).EmailAddress().WithMessage(IncorrentEmail));
            RuleFor(x => x.Password).NotNull().WithMessage(EmptyPassword);
            RuleFor(x => x.Password).NotEmpty().WithMessage(EmptyPassword);
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

        public const string EmptyEmail = "Логин не может быть пусты";
        public const string IncorrentEmail = "Логин некорректный";
        public const string EmptyPassword = "Пароль не может быть пусты";
    }
}
