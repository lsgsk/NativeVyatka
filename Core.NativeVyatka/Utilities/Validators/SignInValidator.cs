using Abstractions.Interfaces.Utilities.Validators;
using FluentValidation;

namespace NativeVyatkaCore.Utilities.Validators
{
    public class SignInValidator : AbstractValidator<string>, ISignInValidator
    {
        public SignInValidator()
        {
        }
        public void VerifyEmailAndPassword(string email, string password)
        {            
        }
    }
}
