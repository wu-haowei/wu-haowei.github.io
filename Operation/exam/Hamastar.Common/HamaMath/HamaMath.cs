using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamastar.Common.HamaMath
{
    public class HamaMath
    {
        public static string BytesToString(float iFileSize)
        {
            string FileSize = string.Empty;
            if (iFileSize > 1024 * 1024)
            {

                FileSize =  ConvertValue((iFileSize / (1024 * 1024)), 2) + " MB";
            }
            else if (iFileSize > 1024)
            {
                FileSize =  ConvertValue((iFileSize / (1024)), 2) + " KB";
            }
            else
            {
                FileSize =  ConvertValue(iFileSize, 2) + " byte";
            }
            return FileSize;
        }

        private static string ConvertValue(float Value, int position)
        {
            if (Value == 0) return "0";
            string sResult = string.Empty;
            sResult = Value.ToString();
            try
            {
                sResult = sResult.Substring(0, sResult.IndexOf('.') + position + 1);
            }
            catch
            {
            }
            return sResult;
        }
    }
}
