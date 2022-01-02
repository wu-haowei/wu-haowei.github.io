using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Hamastar.Common.Security
{
    class Rsa
    {
        private RSACryptoServiceProvider _rsa = null;

        public Rsa()
        {
            _rsa = new RSACryptoServiceProvider();
        }
        
        /// <summary> 
        /// RSA 加密字串 
        /// </summary> 
        /// <param name="original">原始字串</param> 
        /// <param name="xmlString">公鑰 xml 字串</param> 
        /// <returns></returns> 
        public static string Encrypt(string original, string xmlString)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlString);
                byte[] s = Encoding.ASCII.GetBytes(original);
                return BitConverter.ToString(rsa.Encrypt(s, false)).Replace("-", string.Empty);
            }
            catch { return original; }
        }

        /// <summary> 
        /// RSA 加密字串 
        /// 加密後為 256 Bytes Hex String (128 Byte) 
        /// </summary> 
        /// <param name="original">原始字串</param> 
        /// <param name="parameters">公鑰 RSAParameters 類別</param> 
        /// <returns></returns> 
        public static string Encrypt(string original, RSAParameters parameters)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(parameters);
                byte[] s = Encoding.ASCII.GetBytes(original);
                return BitConverter.ToString(rsa.Encrypt(s, false)).Replace("-", string.Empty);
            }
            catch { return original; }
        }

        /// <summary> 
        /// RSA 解密字串 
        /// </summary> 
        /// <param name="hexString">加密後 Hex String</param> 
        /// <param name="xmlString">私鑰 xml 字串</param> 
        /// <returns></returns> 
        public static string Decrypt(string hexString, string xmlString)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlString);
                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                return Encoding.ASCII.GetString(rsa.Decrypt(s, false));
            }
            catch { return hexString; }
        }

        /// <summary> 
        /// RSA 解密字串 
        /// </summary> 
        /// <param name="hexString">加密後 Hex String</param> 
        /// <param name="parameters">私鑰 RSAParameters 類別</param> 
        /// <returns></returns> 
        public static string Decrypt(string hexString, RSAParameters parameters)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(parameters);
                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                return Encoding.ASCII.GetString(rsa.Decrypt(s, false));
            }
            catch { return hexString; }
        } 

        /// <summary>
        /// 公鑰 xml 字串   
        /// </summary>
        /// <returns></returns>
        public string GetPublicXmlString()
        {
            return _rsa.ToXmlString(false);                   
        }
 
        /// <summary>
        /// 私鑰 xml 字串   
        /// </summary>
        /// <returns></returns>
        public string GetPrivateXmlString()
        {
            return _rsa.ToXmlString(true);                   
        }

        /// <summary>
        /// 公鑰
        /// </summary>
        /// <returns></returns>
        public RSAParameters GetPublicParameter()
        {
            return _rsa.ExportParameters(false);            
        }

        /// <summary>
        /// 私鑰
        /// </summary>
        /// <returns></returns>
        public RSAParameters GetPrivateParameter()
        {
            return _rsa.ExportParameters(true);            
        }

    }
}
