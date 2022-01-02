using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Hamastar.Common.IO
{
    public class FileTools
    {
        public static string ConvertSizeFormat(long size)
        {
            double s = size;

            string[] format = new string[] { "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };

            int i = 0;
            while (i < format.Length && s >= 1024)
            {
                s = (int)(100 * s / 1024) / 100.0;
                i++;
            }

            return string.Format(format[i], s); 
        }

        public static string Rename(string OldFileName)
        {
            string result = string.Empty;

            string pattern = @".\((\d)\)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(OldFileName);

            int Num = 1;
            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                string key = match.Groups[1].Value;
                int.TryParse(key, out Num);
                Num++;

                OldFileName = OldFileName.Replace("(" + key + ")", string.Empty);
            }

            result = OldFileName + "(" + Num + ")";

            return result;
        }

        private static string NewFileName = string.Empty;
        public static string GetNewFileName(string OldFileName, string Extension)
        {
            FileInfo FileInfo = new FileInfo(OldFileName + Extension);
            if (!FileInfo.Exists)
            {
                NewFileName = Path.GetFileName(OldFileName);
            }
            else
            {
                GetNewFileName(Rename(OldFileName), Extension);
            }

            return NewFileName;
        }

        # region 檔案排序
        public class MyFileSorter : IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                FileInfo xInfo = (FileInfo)x;
                FileInfo yInfo = (FileInfo)y;


                //依名稱排序
                return xInfo.FullName.CompareTo(yInfo.FullName);//遞增
                //return yInfo.FullName.CompareTo(xInfo.FullName);//遞減

                //依修改日期排序
                //return xInfo.LastWriteTime.CompareTo(yInfo.LastWriteTime);//遞增
                //return yInfo.LastWriteTime.CompareTo(xInfo.LastWriteTime);//遞減
            }
            #endregion
        }
        # endregion
    }


    
}
