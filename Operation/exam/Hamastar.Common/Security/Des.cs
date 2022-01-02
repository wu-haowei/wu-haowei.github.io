using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hamastar.Common.Security
{
    public static class Des
    {
        /// <summary> 
        /// DES 加密字串(注意:意外處理,請寫在自己專案的Tools函式中)
        /// </summary> 
        /// <param name="original">原始字串</param> 
        /// <param name="key">Key，長度必須為 8 個 ASCII 字元</param> 
        /// <param name="iv">IV，長度必須為 8 個 ASCII 字元</param> 
        /// <returns></returns> 
        public static string Encrypt(string original, string key, string iv)
        {
            //try
            //{
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);
                byte[] s = Encoding.UTF8.GetBytes(original);
                ICryptoTransform desencrypt = des.CreateEncryptor();
                return BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty);
            //}
            //catch { return string.Empty; }
        }

        /// <summary> 
        /// DES 解密字串(注意:意外處理,請寫在自己專案的Tools函式中)
        /// </summary> 
        /// <param name="hexString">加密後 Hex String</param> 
        /// <param name="key">Key，長度必須為 8 個 ASCII 字元</param> 
        /// <param name="iv">IV，長度必須為 8 個 ASCII 字元</param> 
        /// <returns></returns> 
        public static string Decrypt(string hexString, string key, string iv)
        {
            //try
            //{
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);

                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                ICryptoTransform desencrypt = des.CreateDecryptor();
                return Encoding.UTF8.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));
            //}
            //catch { return string.Empty; }
        }
    }
}
