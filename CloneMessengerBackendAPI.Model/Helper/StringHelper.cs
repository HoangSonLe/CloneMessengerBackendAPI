using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Model.Helper
{
    public static class StringHelper
    {
        public static byte[] ToMD5Byte(this string text)
        {
            MD5 md5 = MD5.Create();
            md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(text));
            byte[] result = md5.Hash;
            return result;
        }
    }
}
