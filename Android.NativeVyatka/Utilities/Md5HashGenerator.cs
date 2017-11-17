using Abstractions.Interfaces.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace NativeVyatkaCore.Utilities
{
    public class Md5HashGenerator : IMd5HashGenerator
    {
        public string GenerateHash(string value)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = new UTF8Encoding().GetBytes(value);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
