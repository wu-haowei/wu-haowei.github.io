using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;

namespace Hamastar.Common.Weather
{
    /// <summary>
    /// TaiwanWeather 的摘要描述
    /// </summary>
    public class TaiwanWeather
    {
        private string areaurl = "";
        private string html = "";
        private string hostname = "http://www.cwb.gov.tw";
        private ArrayList area = null;
        private int areaindex;
        
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="areaindex">int型態, ex:(int)TaiwanWeather.Area.台北市</param>
        public TaiwanWeather(int areaindex)
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
            
            CreateArea();
            this.areaindex = areaindex;
            this.areaurl = GetAreaUrl(areaindex);
            this.html = Hamastar.Common.Net.Html.GetHtml(this.areaurl);
        }

        private void CreateArea()
        {
            area = new ArrayList();
            area.Add(hostname + "/V7/forecast/taiwan/Taipei_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/New_Taipei_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Taichung_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Tainan_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Kaohsiung_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Keelung_North_Coast.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Taoyuan_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Hsinchu_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Miaoli_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Changhua_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Nantou_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Yunlin_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Chiayi_City.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Pingtung_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Hengchun_Peninsula.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Yilan_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Hualien_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Taitung_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Penghu_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Kinmen_County.htm");
            area.Add(hostname + "/V7/forecast/taiwan/Matsu.htm");
        }

        public enum Area
        {
            台北市,
            新北市,
            台中市,
            台南市,
            高雄市,
            基隆北海岸,
            桃園,
            新竹,
            苗栗,
            彰化,
            南投,
            雲林,
            嘉義,
            屏東,
            恆春半島,
            宜蘭,
            花蓮,
            台東,
            澎湖,
            金門,
            馬祖
        }

        private string GetAreaUrl(int areaindex)
        {
            return area[areaindex].ToString();
        }

        
        /// <summary>
        /// 取得溫度
        /// </summary>
        /// <returns>取得溫度</returns>
        public string GetTemperature()
        {            
            Regex r = new Regex("<td>(.*?)</td>");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                return mcl[0].Value.Replace("<td>", string.Empty).Replace("</td>", string.Empty);
            }
            else
            {
                return "";
            }            
        }

        /// <summary>
        /// 取得降雨機率
        /// </summary>
        /// <returns>取得降雨機率</returns>
        public string GetToRainProbability()
        {
            Regex r = new Regex("<td class=\"num\">(.*?)</td>");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                return mcl[1].Value.Replace("<td class=\"num\">", string.Empty).Replace("</td>", string.Empty);
            }
            else
            {
                return "";
            }            
        }

        /// <summary>
        /// 取得氣象Flash位置
        /// </summary>
        /// <returns>取得氣象Flash位置</returns>
        public string GetWeatherFlashUrl()
        {
            Regex r = new Regex("<param name=\"movie\" value=\"(.*?)\" />");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                return hostname + mcl[0].Value.Replace("<param name=\"movie\" value=\"", string.Empty).Replace("\" />", string.Empty);  
            }
            else
            {
                return "";
            }       
        }

        /// <summary>
        /// 取得氣象Flash標題
        /// </summary>
        /// <returns>取得氣象Flash標題</returns>
        public string GetWeatherFlashTitle()
        {
            Regex r = new Regex("</embed>\n(.*?)\n</object>");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                return mcl[0].Value.Replace("</embed>", string.Empty).Replace("</object>", string.Empty).Replace("\n", string.Empty);
            }
            else
            {
                return "";
            }       
        }

        /// <summary>
        /// 取得舒適度
        /// </summary>
        /// <returns>取得舒適度</returns>
        public string GetComfortable()
        {
            Regex r = new Regex("<td>(.*?)</td>");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                return mcl[0].Value.Replace("<td>", string.Empty).Replace("</td>", string.Empty);
            }
            else
            {
                return "";
            }       
        }

        /// <summary>
        /// 取得氣象資料來源網址
        /// </summary>
        /// <returns>取得氣象資料來源網址</returns>
        public string GetWeatherURL()
        {
            return this.areaurl;
        }

        /// <summary>
        /// 取得氣象資料全部來源網址
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllWeatherURL()
        {
            return area;
        }
        
        /// <summary>
        /// 取得氣象地區名稱
        /// </summary>
        /// <returns>取得氣象地區名稱</returns>
        public string GetAreaName()
        {
            Area area = (Area)Enum.ToObject(typeof(Area), this.areaindex);
            return area.ToString();
        }

        /// <summary>
        /// 取得氣象地區全部名稱
        /// </summary>
        /// <returns>取得氣象地區全部名稱</returns>
        public ArrayList GetAllAreaName()
        {            
            ArrayList areaname = new ArrayList();

            foreach (string en in Enum.GetNames(typeof(Area)))
            {
                areaname.Add(en);
            }

            return areaname;
        }

        /// <summary>
        /// 取得氣象FLASH Object
        /// </summary>
        /// <returns>取得氣象FLASH Object</returns>
        public string GetWeatherFlashObject()
        {
            int width = 32;
            int height = 32;
            StringBuilder  sb = new StringBuilder();
            sb.Append("<object width=\"" + Convert.ToString(width) + "\" height=\"" + Convert.ToString(height) + "\" title=\"" + this.GetWeatherFlashTitle() + "\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0\">\n");
            sb.Append("<param name=\"movie\" value=\""+ this.GetWeatherFlashUrl()  +"\">\n");
            sb.Append("<embed src=\"" + this.GetWeatherFlashUrl() + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"" + Convert.ToString(width) + "\" height=\"" + Convert.ToString(height) + "\" wmode=\"transparent\">\n");
            sb.Append("</embed>" + this.GetWeatherFlashTitle() + "\n");
            sb.Append("</object>\n");

            return sb.ToString();
        }

        /// <summary>
        /// 取得氣象自訂FLASH Object
        /// </summary>
        /// <param name="width">自訂寬度</param>
        /// <param name="height">自訂高度</param>
        /// <returns>取得氣象自訂FLASH Object</returns>
        public string GetWeatherFlashObject(int width, int height)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("<object width=\"" + Convert.ToString(width) + "\" height=\"" + Convert.ToString(height) + "\" title=\"" + this.GetWeatherFlashTitle() + "\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0\">\n");
            sb.Append("<param name=\"movie\" value=\"" + this.GetWeatherFlashUrl() + "\">\n");
            sb.Append("<embed src=\"" + this.GetWeatherFlashUrl() + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"" + Convert.ToString(width) + "\" height=\"" + Convert.ToString(height) + "\" wmode=\"transparent\">\n");
            sb.Append("</embed>" + this.GetWeatherFlashTitle() + "\n");
            sb.Append("</object>\n");

            return sb.ToString();
        }

        /// <summary>
        /// 取得氣象圖片
        /// </summary>
        /// <returns>取得氣象圖片</returns>
        public string GetWeatherImageObject()
        {
            Regex r = new Regex("<td>\n(.*?)</td>");
            MatchCollection mcl = r.Matches(this.html);

            if (mcl.Count > 0)
            {
                string temp = mcl[0].Value.Replace("<td>\n\t\t<img src=\"", string.Empty).Replace("</td>", string.Empty);
                temp = "<img src=\"" + this.hostname + temp.Replace("../..", "/V7");
                return temp;
            }
            else
            {
                return "";
            }
        }

    }

}