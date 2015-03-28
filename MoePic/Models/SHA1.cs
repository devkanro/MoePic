using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoePic.Models
{
    /// <summary>
    /// 提供快速的对字符串进行 SHA1 算法的 HASH 方案
    /// </summary>
    public static class SHA1
    {
        public static string GetSHA1(string hashString)
        {
            System.Security.Cryptography.SHA1Managed sha = new System.Security.Cryptography.SHA1Managed();
            return Windows.Security.Cryptography.CryptographicBuffer.
                EncodeToHexString(
                Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(
                sha.ComputeHash(
                Encoding.UTF8.GetBytes(
                hashString
                ))));
        }
    }
}
