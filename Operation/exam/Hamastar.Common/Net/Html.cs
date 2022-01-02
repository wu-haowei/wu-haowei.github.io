using System;
using System.Data;
using System.Net;
using System.IO;
using System.Text;

namespace Hamastar.Common.Net
{
    /// <summary>
    /// Html 的摘要描述
    /// </summary>
    public class Html
    {
        public Html()
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
        }

        /// <summary>
        /// 取得網頁原始碼
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String GetHtml(String url)
        {
            return GetHtml(url, Encoding.UTF8);
        }
        /// <summary>
        /// 依編碼方式及起訖Tag取得網頁原始碼
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Code"></param>
        /// <param name="StarTag"></param>
        /// <param name="EndTag"></param>
        /// <returns></returns>
        public static String GetHtml(String url, Encoding Code, string StarTag, string EndTag)
        {
            String htmltag = "";
            htmltag = GetHtml(url, Code);
            int iSPos = htmltag.IndexOf(StarTag);
            int iEPos = htmltag.LastIndexOf(EndTag);
            if (iSPos != -1)
                htmltag = htmltag.Substring(iSPos + StarTag.Length, iEPos - iSPos - StarTag.Length);
            return htmltag;
        }

        /// <summary>
        /// 依編碼方式取得網頁原始碼
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static String GetHtml(String url, Encoding Code)
        {
            String htmltag = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Code);           
            htmltag = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            return htmltag;
        }
    }
}