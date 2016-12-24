namespace Abstractions.Interfaces.Utilities.Validators
{
    public interface ISignInValidator
    {
        void VerifyEmailAndPassword(string email, string password);
    }
}
