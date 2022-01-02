using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hamastar.Common
{
    public static class Expansion
    {
        #region checkID 驗證身份證字號
        /// <summary>
        /// 驗證身份證字號
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool isIdentificationId(this string id)
        {
            id = id.ToUpper();
            // 使用「正規表達式」檢驗格式 [A~Z] {1}個數字 [0~9] {9}個數字
            var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
            if (!regex.IsMatch(id))
            {
                //Regular Expression 驗證失敗，回傳 ID 錯誤
                return false;
            }

            //除了檢查碼外每個數字的存放空間 
            int[] seed = new int[10];

            //建立字母陣列(A~Z)
            //A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            //P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35            
            string[] charMapping = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            string target = id.Substring(0, 1); //取第一個英文數字
            for (int index = 0; index < charMapping.Length; index++)
            {
                if (charMapping[index] == target)
                {
                    index += 10;
                    //10進制的高位元放入存放空間   (權重*1)
                    seed[0] = index / 10;

                    //10進制的低位元*9後放入存放空間 (權重*9)
                    seed[1] = (index % 10) * 9;

                    break;
                }
            }
            for (int index = 2; index < 10; index++) //(權重*8~1)
            {   //將剩餘數字乘上權數後放入存放空間                
                seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);
            }
            //檢查是否符合檢查規則，10減存放空間所有數字和除以10的餘數的個位數字是否等於檢查碼            
            //(10 - ((seed[0] + .... + seed[9]) % 10)) % 10 == 身分證字號的最後一碼   
            if ((10 - (seed.Sum() % 10)) % 10 != Convert.ToInt32(id.Substring(9, 1)))
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 外來人口統一證號驗證(舊版本)
        /// </summary>
        /// <param name="numberToCheck">要驗證的證號</param>
        /// <returns>驗證結果</returns>
        public static bool CheckForeignIdNumber(this string numberToCheck)
        {
            numberToCheck = numberToCheck.ToUpper();
            if (numberToCheck.Substring(1, 1).IsNumeric())
                return CheckForeignIdNumber2(numberToCheck);

            // 基礎檢查 「任意1個字母」+「A~D其中一個字母」+「8個數字」
            if (!Regex.IsMatch(numberToCheck, @"^[A-Za-z][A-Da-d]\d{8}$")) return false;

            // 縣市區域碼
            var cityCode = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
            // 計算時使用的容器，最後一個位置拿來放檢查碼，所以有11個位置(縣市區碼佔2個位置)
            var valueContainer = new int[11];
            valueContainer[0] = cityCode[numberToCheck[0] - 65] / 10; // 區域碼十位數
            valueContainer[1] = cityCode[numberToCheck[0] - 65] % 10; // 區域碼個位數
            valueContainer[2] = cityCode[numberToCheck[1] - 65] % 10; // 性別碼個位數
                                                                      // 證號執行特定數規則所產生的結果值的加總，這裡把初始值訂為區域碼的十位數數字(特定數為1，所以不用乘)
            var sumVal = valueContainer[0];

            // 迴圈執行特定數規則
            for (var i = 1; i <= 9; i++)
            {
                // 跳過性別碼，如果是一般身分證字號則不用跳過
                if (i > 1)
                    // 將當前證號於索引位置的數字放到容器的下一個索引的位置
                    valueContainer[i + 1] = numberToCheck[i] - 48;

                // 特定數為: 1987654321 ，因為首個數字1已經在sumVal初始值算過了，所以這裡從9開始
                sumVal += valueContainer[i] * (10 - i);
            }

            // 此為「檢查碼 = 10 - 總和值的個位數數字 ; 若個位數為0則取0為檢查碼」的反推
            if ((sumVal + valueContainer[10]) % 10 == 0) return true;
            return false;
        }

        /// <summary>
        /// 外來人口統一證號驗證(2020/09月的新版本)
        /// </summary>
        /// <param name="numberToCheck">要驗證的證號</param>
        /// <returns>驗證結果</returns>
        public static bool CheckForeignIdNumber2(string id)
        {
            bool check = false;
            id = id.ToUpper();
            // 使用「正規表達式」檢驗格式 [A~Z] {1}個數字 [0~9] {9}個數字
            var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
            if (!regex.IsMatch(id))
            {
                //Regular Expression 驗證失敗，回傳 ID 錯誤
                return false;
            }
            #region 第一碼對應的數字
            int first_map_num = 0;
            switch (id.Substring(0, 1))
            {
                case "A":
                    first_map_num = 10;
                    break;
                case "B":
                    first_map_num = 11;
                    break;
                case "C":
                    first_map_num = 12;
                    break;
                case "D":
                    first_map_num = 13;
                    break;
                case "E":
                    first_map_num = 14;
                    break;
                case "F":
                    first_map_num = 15;
                    break;
                case "G":
                    first_map_num = 16;
                    break;
                case "H":
                    first_map_num = 17;
                    break;
                case "J":
                    first_map_num = 18;
                    break;
                case "K":
                    first_map_num = 19;
                    break;
                case "L":
                    first_map_num = 20;
                    break;
                case "M":
                    first_map_num = 21;
                    break;
                case "N":
                    first_map_num = 22;
                    break;
                case "P":
                    first_map_num = 23;
                    break;
                case "Q":
                    first_map_num = 24;
                    break;
                case "R":
                    first_map_num = 25;
                    break;
                case "S":
                    first_map_num = 26;
                    break;
                case "T":
                    first_map_num = 27;
                    break;
                case "U":
                    first_map_num = 28;
                    break;
                case "V":
                    first_map_num = 29;
                    break;
                case "X":
                    first_map_num = 30;
                    break;
                case "Y":
                    first_map_num = 31;
                    break;
                case "W":
                    first_map_num = 32;
                    break;
                case "Z":
                    first_map_num = 33;
                    break;
                case "I":
                    first_map_num = 34;
                    break;
                case "O":
                    first_map_num = 35;
                    break;

            }
            #endregion
            if (first_map_num == 0)
                return false;

            #region 計算每個號碼的基數加總
            char[] chars = id.ToCharArray();
            int total = 0;
            int startnum = 8;
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    int temp1 = Convert.ToInt32(first_map_num.ToString().Substring(0, 1)) * 1;
                    if (temp1.ToString().Length > 1)
                        total += Convert.ToInt32(temp1.ToString().Substring(1, 1));
                    else
                        total += Convert.ToInt32(temp1.ToString().Substring(0, 1));

                    int temp2 = Convert.ToInt32(first_map_num.ToString().Substring(1, 1)) * 9;
                    if (temp2.ToString().Length > 1)
                        total += Convert.ToInt32(temp2.ToString().Substring(1, 1));
                    else
                        total += Convert.ToInt32(temp2.ToString().Substring(0, 1));
                }
                else
                {
                    int temp3 = Convert.ToInt32(chars[i].ToString()) * startnum;
                    if (temp3.ToString().Length > 1)
                        total += Convert.ToInt32(temp3.ToString().Substring(1, 1));
                    else
                        total += Convert.ToInt32(temp3.ToString().Substring(0, 1));
                    startnum--;
                }
            }
            #endregion
            //當個位數不為零時，以 10 減基數之個位數即得檢查碼；當個位數為零時，則即以零為檢查碼。
            if (total != 0)
                total = 10 - total;
            if (total == Convert.ToInt32(chars[9].ToString()))
                check = true;
            return check;
        }

        #region 取得列舉的desc
        /// <summary>
        /// 取得列舉的desc
        /// </summary>
        /// <param name="value">列舉值</param>
        /// <returns></returns>
        public static string ToDescriptionString(this System.Enum value)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        #endregion

        #region 判斷字串是否可以轉換成數字
        /// <summary>
        /// 判斷字串是否可以轉換成數字
        /// </summary>
        /// <param name="theValue"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }
        #endregion

        #region 取得日期相差數
        /// <summary>
        /// 取得日期相差數
        /// Datepart implemented:
        ///     "year" (abbr. "yy", "yyyy"),
        ///     "quarter" (abbr. "qq", "q"),
        ///     "month" (abbr. "mm", "m"),
        ///     "day" (abbr. "dd", "d"),
        ///     "week" (abbr. "wk", "ww"),
        ///     "hour" (abbr. "hh"),
        ///     "minute" (abbr. "mi", "n"),
        ///     "second" (abbr. "ss", "s"),
        ///     "millisecond" (abbr. "ms").
        /// </summary>
        /// <param name="DatePart">要比較的類型 </param>
        /// <param name="EndDate">結束日</param>
        /// <returns></returns>
        public static Int64 DateDiff(this DateTime StartDate, String DatePart, DateTime EndDate)
        {
            Int64 DateDiffVal = 0;
            System.Globalization.Calendar cal = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar;
            TimeSpan ts = new TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (DatePart.ToLower().Trim())
            {
                #region year

                case "year":
                case "yy":
                case "yyyy":
                    DateDiffVal = (Int64)(cal.GetYear(EndDate) - cal.GetYear(StartDate));
                    break;

                #endregion year

                #region quarter

                case "quarter":
                case "qq":
                case "q":
                    DateDiffVal = (Int64)((((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 4)
                                        + ((cal.GetMonth(EndDate) - 1) / 3))
                                        - ((cal.GetMonth(StartDate) - 1) / 3));
                    break;

                #endregion quarter

                #region month

                case "month":
                case "mm":
                case "m":
                    DateDiffVal = (Int64)(((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 12
                                        + cal.GetMonth(EndDate))
                                        - cal.GetMonth(StartDate));
                    break;

                #endregion month

                #region day

                case "day":
                case "d":
                case "dd":
                    DateDiffVal = (Int64)ts.TotalDays;
                    break;

                #endregion day

                #region week

                case "week":
                case "wk":
                case "ww":
                    DateDiffVal = (Int64)(ts.TotalDays / 7);
                    break;

                #endregion week

                #region hour

                case "hour":
                case "hh":
                    DateDiffVal = (Int64)ts.TotalHours;
                    break;

                #endregion hour

                #region minute

                case "minute":
                case "mi":
                case "n":
                    DateDiffVal = (Int64)ts.TotalMinutes;
                    break;

                #endregion minute

                #region second

                case "second":
                case "ss":
                case "s":
                    DateDiffVal = (Int64)ts.TotalSeconds;
                    break;

                #endregion second

                #region millisecond

                case "millisecond":
                case "ms":
                    DateDiffVal = (Int64)ts.TotalMilliseconds;
                    break;

                #endregion millisecond

                default:
                    throw new Exception(String.Format("DatePart \"{0}\" is unknown", DatePart));
            }
            return DateDiffVal;
        }
        #endregion

        #region 取得日期是星期幾
        /// <summary>
        /// 取得日期是星期幾
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns></returns>
        public static string ToTaiwanFormatWeekDay(this DateTime dt)
        {
            string w = dt.DayOfWeek.ToString("d");
            string strweek = "星期";
            switch (w)
            {
                case "0":
                    strweek += "日";
                    break;
                case "1":
                    strweek += "一";
                    break;
                case "2":
                    strweek += "二";
                    break;
                case "3":
                    strweek += "三";
                    break;
                case "4":
                    strweek += "四";
                    break;
                case "5":
                    strweek += "五";
                    break;
                case "6":
                    strweek += "六";
                    break;
            }
            return strweek;
        }
        #endregion

        #region 判斷是否為日期

        /// <summary>
        /// 判斷是否為日期
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDate(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                DateTime dt;
                return (DateTime.TryParse(input, out dt));
            }
            else
            {
                return false;
            }
        }

        #endregion 判斷是否為日期

        #region 檢查是否有全形字
        /// <summary>
        /// 檢查是否有全形字
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool FullWidthWord(this String values)
        {
            bool result = false;
            string pattern = @"^[\u4E00-\u9fa5]+$";
            foreach (char item in values)
            {
                //以Regex判斷是否為中文字，中文字視為全形
                if (!Regex.IsMatch(item.ToString(), pattern))
                {
                    //以16進位值長度判斷是否為全形字
                    if (string.Format("{0:X}", Convert.ToInt32(item)).Length != 2)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion 檢查是否有全形字

        #region 取字串右邊幾個字

        /// <summary>
        /// 取字串右邊幾個字
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string s, int length)
        {
            length = Math.Max(length, 0);

            if (s.Length > length)
            {
                return s.Substring(s.Length - length, length);
            }
            else
            {
                return s;
            }
        }

        #endregion 取字串右邊幾個字

        #region 取得字串左邊幾個字

        /// <summary>
        /// 取得字串左邊幾個字
        /// </summary>
        /// <param name="s"> /param>
        /// <param name="length"> </param>
        /// <returns></returns>
        public static string Left(this string s, int length)
        {
            length = Math.Max(length, 0);

            if (s.Length > length)
            {
                return s.Substring(0, length);
            }
            else
            {
                return s;
            }
        }

        #endregion 取得字串左邊幾個字

        /// <summary>
        /// 取得隨機碼
        /// </summary>
        /// <param name="maxSize">要幾個</param>
        /// <param name="OnlyUpperEng">是否要只要大寫的隨機碼</param>
        /// <returns></returns>
        public static string CreateRandomCode(int maxSize, bool OnlyUpperEng)
        {
            char[] chars = null;
            if (!OnlyUpperEng)
            {
                chars = new char[62];
                chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            }
            else
            {
                chars = new char[26];
                chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            }
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

        #region 取得金鑰
        /// <summary>
        /// 取得金鑰
        /// </summary>
        /// <returns></returns>
        public static string GetTokenKey()
        {
            return string.Format("{0}{1}{2}", CreateRandomCode(1, true), CreateRandomCode(8, false).ToLower(), CreateRandomCode(1, true));
        }
        #endregion
        /// <summary>
        /// 將 DateTime 物件格式化成yyyy-MM-dd日期字串.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToYmdDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///  將 DateTime 物件格式化成日期字串yyyyMMdd.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToYmdDateNoSlashString(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }


        /// <summary>
        /// 將 DateTime 物件格式化成台灣日期字串yyyy年MM月dd日.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToTaiwanYmdDateString(this DateTime date)
        {
            return date.ToString("yyyy年MM月dd日");
        }

        // 將 DateTime 物件格式化成yyyy-MM-dd HHmmss日期時間字串.
        /// <summary>
        /// 將 DateTime 物件格式化成yyyy-MM-dd HHmmss日期時間字串.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToYmdDHHmmssateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 將 DateTime 物件轉成西元年
        /// <summary>
        /// 將 DateTime 物件格式化成yyyy-MM-dd 日期字串.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToEngYmdString(this string date)
        {

            string[] DateArr = date.Split('-');
            date = string.Format("{0}-{1}", (Convert.ToInt32(DateArr[0]) + 1911), date.Substring(DateArr[0].Length + 1));
            return date.Replace("-", "/");
        }

        // 將 DateTime 物件轉成民國年
        /// <summary>
        /// 將 DateTime 物件格式化成yyyy-MM-dd 日期字串.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToTWYmdString(this string date)
        {

            string[] DateArr = date.Split('/');
            date = string.Format("{0}/{1}", (Convert.ToInt32(DateArr[0]) - 1911), date.Substring(DateArr[0].Length + 1));
            return date.Replace("/", "-");
        }

        #region 正規表述式

        /// <summary>
        /// 檢查字元是否為大寫字母
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAlphaAZ(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^A-Z]"))
                ? true : false;
        }

        /// <summary>
        /// 檢查字元是否為小寫字母
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAlphaaz(String InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^a-z]"))
                ? true : false;
        }

        /// <summary>
        /// 檢查字元是否為大小寫字母
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAlphaaZ(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^a-zA-Z]"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為數字或者字母
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^a-zA-Z0-9]"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為正整數包括零
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^0-9]"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為合法email
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsEmail(this string InputString)
        {
            Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            return (InputString != string.Empty && regex.IsMatch(InputString))
                ? true : false;
        }

        /// <summary>
        /// 檢查輸入數字是否位數符合
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="median"></param>
        /// <returns></returns>
        public static bool IsNumberMedianMatch(this string InputString, int median)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^\d{" + median.ToString() + "}$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查輸入數字是否最少為n位數
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="median">n</param>
        /// <returns></returns>
        public static bool IsNumberMedianMatchlast(this string InputString, int median)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^\d{" + median.ToString() + ",}$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查輸入數字是否在MAX到MIN位數
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="Max">最大位數</param>
        /// <param name="Min">最小位數</param>
        /// <returns></returns>
        public static bool IsNumberInRange(this string InputString, int Max, int Min)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^\d{" + Max.ToString() + "," + Min.ToString() + "}$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為金額 小數固定為兩位
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAmount(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^[0-9]+(.[0-9]{2})?$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為金額 小數為一位或兩位
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsAmountLast(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^[0-9]+(.[0-9]{1,2})?$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為非零的正整數
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^\+?[1-9][0-9]*$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為非零的負整數
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsNegativeInteger(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^\-?[1-9][0-9]*$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查字串是否符合長度
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="median"></param>
        /// <returns></returns>
        public static bool IsStringMatchLength(this string InputString, int median)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^.{" + median.ToString() + "}$"))
                ? true : false;
        }

        /// <summary>
        /// 是否為符合型式的密碼,字母開頭,長度6~18,字母及數字和下底線混合
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsValidPassword(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^[a-zA-Z]\w{5,17}$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為網址
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsWebSite(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^(http|ftp|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為月份 01~09 , 1~12
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsMonth(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^(0?[1-9]|1[0-2])$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為日期 01~09 , 1~31
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsDay(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^((0?[1-9])|((1|2)[0-9])|30|31)$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否有下列符號 ^%&’,;=?$\"
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsDelimiter(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "[^%&’,;=?$\x22]+"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否有逗點分割的字串
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static bool IsDotString(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, @"^([\w\d\u4e00-\u9fa5],?)+$"))
                ? true : false;
        }

        /// <summary>
        /// 檢查是否為手機號碼.
        /// </summary>
        /// <param name="InputString">The input email.</param>
        /// <returns>
        /// 	<c>true</c> if [is cell phone] [the specified input email]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCellPhone(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^[0-9]{4}[0-9]{3}[0-9]{3}"))
                ? true : false;
        }
        /// <summary>
        /// 檢查是否為英數字.
        /// </summary>
        /// <param name="InputString">The input email.</param>
        /// <returns>
        /// 	<c>true</c> if [is cell phone] [the specified input email]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumberOrWord(this string InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^[A-Za-z0-9]+$"))
                ? true : false;
        }


        #endregion 正規表述式



    }
}
