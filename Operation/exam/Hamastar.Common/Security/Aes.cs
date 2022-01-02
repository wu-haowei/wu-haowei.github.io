using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Hamastar.Common.Security
{
    public static class Aes
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Encrypt(string AesKey, string Value)
        {
            //加解密需32位密鑰(中文佔3bytes)
            //Byte[] keyArray = UTF8Encoding.UTF8.GetBytes(WebConfig.AesKey);
            Byte[] keyArray = GetFixBytes(32, AesKey);
            Byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(Value);

            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyArray;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string EncryptWithUrlEncode(string AesKey, string Value)
        {
            return HttpUtility.UrlEncode(Encrypt(AesKey, Value));
        }

        public static string EncryptCustom(string AesKey, string Value)
        {
            return Encrypt(AesKey, Value).Replace("+", "!").Replace("/", "$").Replace("=", "@");
        }

        public static string DecryptWithUrlEncode(string AesKey, string Value)
        {
            if (Value.Contains("%"))
                return Decrypt(AesKey, HttpUtility.UrlDecode(Value));
            else
                return Decrypt(AesKey, Value);
        }

        public static string DecryptCustom(string AesKey, string Value)
        {
            return Decrypt(AesKey, Value.Replace("!", "+").Replace("$", "/").Replace("@", "="));
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Decrypt(string AesKey, string Value)
        {
            //Byte[] keyArray = UTF8Encoding.UTF8.GetBytes(WebConfig.AesKey);
            Byte[] keyArray = GetFixBytes(32, AesKey);
            Byte[] toEncryptArray = Convert.FromBase64String(Value);

            string a = UTF8Encoding.UTF8.GetString(toEncryptArray);

            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyArray;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 產生固定長度Key
        /// </summary>
        /// <param name="length"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static byte[] GetFixBytes(int length, string PWD)
        {
            byte[] salt = new byte[] { 0x0A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1 };
            Rfc2898DeriveBytes rfcKey = new Rfc2898DeriveBytes(PWD, salt);

            return rfcKey.GetBytes(length);
        }
    }
}
