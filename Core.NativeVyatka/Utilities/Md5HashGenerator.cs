namespace NativeVyatkaCore.Utilities
{
    /*public static class Md5HashGenerator
    {
        public static string GetMd5HashFromString(this string value)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = new UTF8Encoding().GetBytes(value);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                    sb.Append(t.ToString("X2"));
                return sb.ToString();
            }
        }
    }*/
}
