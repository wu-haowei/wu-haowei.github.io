using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamastar.Common.Calendar
{
    public static class Date
    {
        /// <summary>
        /// 取得禮拜幾
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetChineseDayOfWeek(DateTime dt)
        {
            DayOfWeek dayofWeek = dt.DayOfWeek;
            switch(dayofWeek)
            {
                case DayOfWeek.Monday:                    
                    return "一";
                case DayOfWeek.Tuesday:
                    return "二";
                case DayOfWeek.Wednesday:
                    return "三";
                case DayOfWeek.Thursday:
                    return "四";
                case DayOfWeek.Friday:
                    return "五";
                case DayOfWeek.Saturday:
                    return "六";
                case DayOfWeek.Sunday:
                    return "日";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 取得中文月份名稱
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static string GetChineseMonth(int Month)
        {
            string month = string.Empty;
            switch (Month)
            {
                case 1:
                    month = "一";
                    break;
                case 2:
                    month = "二";
                    break;
                case 3:
                    month = "三";
                    break;
                case 4:
                    month = "四";
                    break;
                case 5:
                    month = "五";
                    break;
                case 6:
                    month = "六";
                    break;
                case 7:
                    month = "七";
                    break;
                case 8:
                    month = "八";
                    break;
                case 9:
                    month = "九";
                    break;
                case 10:
                    month = "十";
                    break;
                case 11:
                    month = "十一";
                    break;
                case 12:
                    month = "十二";
                    break;
                default:
                    break;
            }
            return month;
        }
    }
}
