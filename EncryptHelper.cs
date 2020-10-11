using System.Security.Cryptography;
using System.Text;

namespace CNET_DDNS
{
    public static class EncryptHelper
    {
        public static byte[] Protect(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return ProtectedData.Protect(bytes, null, DataProtectionScope.LocalMachine);
        }

        public static string UnProtect(byte[] data)
        {
            var bytes = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
