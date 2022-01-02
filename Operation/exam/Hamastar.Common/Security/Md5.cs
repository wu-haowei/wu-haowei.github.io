using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hamastar.Common.Security
{    
    public static class Md5
    {
        /// <summary> 
        /// 取得 MD5 編碼後的 Hex 字串 
        /// 加密後為 32 Bytes Hex String (16 Byte) 
        /// </summary> 
        /// <param name="original">原始字串</param> 
        /// <returns></returns> 
        public static string GetString(string original) 
        { 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider(); 
            byte[] b = md5.ComputeHash(Encoding.UTF8.GetBytes(original)); 
            return BitConverter.ToString(b).Replace("-", string.Empty); 
        }
    }
}
