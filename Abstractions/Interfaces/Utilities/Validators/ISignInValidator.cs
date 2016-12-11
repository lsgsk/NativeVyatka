using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Utilities.Validators
{
    public interface ISignInValidator
    {
        void VerifyEmailAndPassword(string email, string password);
    }
}
