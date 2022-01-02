using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace Hamastar.Common
{
    public class chkcode
    {
        public chkcode()
        {
        }
        // 驗證碼的文字長度。
        private static int _randomTextLength = 4;

        // 隨機產生驗證碼所需的文字字串。
        private static string GenerateRandomText()
        {
            return CreateRandomCode(_randomTextLength);
        }
        public static string CreateRandomCode(int maxSize)
        {
            char[] chars = new char[55];
            chars =
            "abcdefghijkmnopqrstuvwxyABCDEFGHIJKLMNPQRSTUVWXY3456789".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        // 呼叫自訂的 GenerateRandomText() 函式來產生驗證碼的文字字串。
        public static string CreateRandomCode()
        {
            string _randomText = GenerateRandomText();
            return _randomText;
        }

        public static void CreateImage(string checkCode , Page _Page)
        {
            if (checkCode == "")
                checkCode = chkcode.CreateRandomCode();

            int iwidth = (int)((checkCode.Length + 1.5) * 11.5);
            int ihight = 25;
            //Validate Code
            Random rd = new Random(unchecked((int)DateTime.Now.Ticks));
            Bitmap Bmp = new Bitmap(iwidth, ihight);  //建立實體圖檔並設定大小
            Graphics Gpi = Graphics.FromImage(Bmp);
            Font Font1 = new Font("Verdana", 14, FontStyle.Italic);

            Pen PenLine = new Pen(Brushes.Red, 1);  //實體化筆刷並設定顏色、大小(畫X,Y軸用)
            Gpi.Clear(Color.White);    //設定背景顏色

            RngRandom oRngRandom = new RngRandom();
           
            Gpi.DrawLine(PenLine, 0, oRngRandom.RND(iwidth), 90, oRngRandom.RND(ihight));
            Gpi.DrawString(checkCode, Font1, Brushes.Black, 0, 0);
            for (int i = 0; i <= ihight; i++)            //亂數產生霧點，擾亂機器人辨別
            {
                int RandPixelX = oRngRandom.RND(0, iwidth);
                int RandPixelY = oRngRandom.RND(0, ihight);
                Bmp.SetPixel(RandPixelX, RandPixelY, Color.Blue);
            }
            _Page.Session["CAPTCHA"] = checkCode;
            _Page.Response.Clear();
            _Page.Response.ContentType = "image/gif";
            Bmp.Save(_Page.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
        }
    }
}
