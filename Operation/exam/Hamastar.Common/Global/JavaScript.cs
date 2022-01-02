using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamastar.Common.Global
{
    public class JavaScript
    {
        public static string GetJavaScript(string Domain)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/jquery-1.7.2.min.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/oka_model.js'></script>");            
            sb.AppendLine("<link rel='stylesheet' type='text/css'  href='" + Domain + "/Scripts/flowplayer-5.4.3/skin/functional.css'></script> ");   
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/flowplayer-5.4.3/flowplayer.min.js'></script>");            
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/jquery.cycle2.min.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/jquery.cycle2.carousel.min.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='" + Domain + "/Scripts/jquery.touchwipe.min.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='https://maps.googleapis.com/maps/api/js?key=AIzaSyAo1MAXwJfl0q3fNU_5QyogEgyecOICpWs&libraries=places,drawing\'></script>");

            //  HighChats & HighMaps 使用 
            sb.AppendLine("<script type='text/javascript'  src='" + Domain + "/Scripts/Chart/highcharts.js'></script> ");
            sb.AppendLine("<script type='text/javascript'  src='" + Domain + "/Scripts/Chart/map.src.js'></script> ");
            sb.AppendLine("<script type='text/javascript'  src='" + Domain + "/Scripts/Chart/highcharts_theme.js'></script> ");
            sb.AppendLine("<script type='text/javascript'  src='" + Domain + "/Scripts/Chart/tw-all.js'></script> ");
            sb.AppendLine("<link rel='stylesheet' type='text/css'  href='" + Domain + "/Scripts/Chart/highcharts.css'></script> ");            


            return sb.ToString();
        }
    }
}
