using Hamastar.BusinessObject;

using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;


/// <summary>
/// Class1 的摘要描述
/// </summary>
public class ReportService
{

    public byte[] ReportProcess(string ReportType, ReportModel parm)
    {
        byte[] result = null;
        HSSFWorkbook workbook = new HSSFWorkbook();//建立活頁簿
        GenStyle(workbook, ref parm);
        switch (ReportType)
        {
            case "審查案件一覽表":
                result = Report1(parm, ReportType, workbook);
                break;
            case "裝置完成統計表":
                result = Report2(parm, ReportType, workbook);
                break;
            case "核銷統計表(名冊)":
                result = Report3(parm, ReportType, workbook);
                break;
            case "合約院所執行表":
                result = Report4(parm, ReportType, workbook);
                break;
            case "滿意度調查表":
                result = Report5(parm, ReportType, workbook);
                break;
        }
        return result;
    }

    #region 樣式
    public void GenStyle(HSSFWorkbook workbook, ref ReportModel parm)
    {
        //標題樣式----------------
        HSSFCellStyle cstitle = (HSSFCellStyle)workbook.CreateCellStyle();//主標題
        //框線樣式及顏色
        cstitle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//副標題
        cstitle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

        HSSFFont font1 = (HSSFFont)workbook.CreateFont();
        font1.FontHeightInPoints = 16;//文字大小
        font1.FontName = "標楷體";
        cstitle.SetFont(font1);
        parm.cstitle = cstitle;
        //資料內容樣式-----------------------
        HSSFCellStyle csdata = (HSSFCellStyle)workbook.CreateCellStyle();//內文
        //框線樣式及顏色
        csdata.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        csdata.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        csdata.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        csdata.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
        csdata.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        csdata.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;//左邊框色
        csdata.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;//右邊框色
        csdata.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        csdata.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直置中
        csdata.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平置中

        HSSFFont font2 = (HSSFFont)workbook.CreateFont();
        font2.FontHeightInPoints = 12;
        font2.FontName = "標楷體";
        csdata.SetFont(font2);
        parm.csdata = csdata;

    }
    #endregion

    #region 審查案件一覽表
    private byte[] Report1(ReportModel parm, string SheetName, HSSFWorkbook workbook)
    {
        byte[] result = null;
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(SheetName);
        sheet.FitToPage = false;
        sheet.PrintSetup.PaperSize = 9;//a4
        int colct = 9;//欄位數

        #region  資料
        //第一列-----------------------------
        sheet.CreateRow(0).CreateCell(0).SetCellValue(SheetName);
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, colct));//跨欄
        sheet.GetRow(0).GetCell(0).CellStyle = parm.cstitle;

        //資料列
        int i = 1;
        DataTable dt = new DataTable();
        foreach (DataRow item in dt.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i);
            sheet.GetRow(i).CreateCell(0).SetCellValue("xxx");
            sheet.GetRow(i).GetCell(0).CellStyle = parm.csdata;
        }
        #endregion

        //設定欄位寬度
        //sheet.SetColumnWidth(0, 13 * 256);
        //for (int x = 3; x < 9; x++)
        //    sheet.SetColumnWidth(x, 7 * 256);

        //sheet.SetColumnWidth(3, 8 * 256);

        sheet.DisplayGridlines = true;
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        result = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return result;

    }

    #endregion



    #region 合併欄位範例
    private CellRangeAddress GetMergedCellAddress(string CellAddress)
    {
        #region EXECL格式說明
        /*
            A 1:B 2
            Column Row:Column Row
            A1:G1 = 0,0,6,0
            B5:B7
            4,6,1,1
        */
        #endregion 
        int firstCol = 0;
        int firstRow = 0;
        int lastCol = 0;
        int lastRow = 0;

        if (!string.IsNullOrEmpty(CellAddress) && CellAddress.IndexOf(':') > -1)//檢查格式是否合法 例: A1:C3為合法格式
        {
            string[] CellAddressArray = CellAddress.ToUpper().Split(':', (char)StringSplitOptions.RemoveEmptyEntries);//切割儲存格範圍字串

            if (CellAddressArray.Length == 2)//檢查格式是否合法 
            {
                int index = 0;
                foreach (var item in CellAddressArray)
                {
                    string RowIndex = string.Empty;//1~65525
                    string ColIndex = string.Empty;//A~ZZ



                    foreach (var item2 in item)
                    {
                        if (char.IsUpper(item2))//把所有英文串在一起 == Column
                        {
                            ColIndex += item2;
                        }
                        if (char.IsNumber(item2))//把所有數字串在一起 == Row
                        {
                            RowIndex += item2;
                        }
                    }
                    if (index == 0)
                    {
                        int.TryParse(RowIndex, out firstRow);
                        int power = ColIndex.Length - 1;



                        foreach (var indexChar in ColIndex)
                        {
                            firstCol += (int)Math.Pow(26, power--) * ((int)indexChar - 64);
                        }
                    }
                    if (index == 1)
                    {
                        int.TryParse(RowIndex, out lastRow);
                        int power = ColIndex.Length - 1;



                        foreach (var indexChar in ColIndex)
                        {
                            lastCol += (int)Math.Pow(26, power--) * ((int)indexChar - 64);
                        }
                    }
                    index++;
                }
            }
        }
        return new CellRangeAddress(firstRow - 1, lastRow - 1, firstCol - 1, lastCol - 1);
    }

    #endregion
    #region 裝置完成統計表
    private byte[] Report2(ReportModel parm, string SheetName, HSSFWorkbook workbook)
    {
        byte[] result = null;
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(SheetName);
        sheet.FitToPage = false;
        sheet.PrintSetup.PaperSize = 9;//a4
        int colct = 9;//欄位數

        #region  資料
        //第一列-----------------------------
        sheet.CreateRow(0).CreateCell(0).SetCellValue(SheetName);
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, colct));//跨欄
        sheet.GetRow(0).GetCell(0).CellStyle = parm.cstitle;

        //資料列
        int i = 1;
        DataTable dt = new DataTable();
        foreach (DataRow item in dt.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i);
            sheet.GetRow(i).CreateCell(0).SetCellValue("xxx");
            sheet.GetRow(i).GetCell(0).CellStyle = parm.csdata;
        }
        #endregion

        //設定欄位寬度
        //sheet.SetColumnWidth(0, 13 * 256);
        //for (int x = 3; x < 9; x++)
        //    sheet.SetColumnWidth(x, 7 * 256);

        //sheet.SetColumnWidth(3, 8 * 256);

        sheet.DisplayGridlines = true;
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        result = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return result;
    }
    #endregion

    #region 核銷統計表(名冊)
    private byte[] Report3(ReportModel parm, string SheetName, HSSFWorkbook workbook)
    {
        byte[] result = null;
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(SheetName);
        sheet.FitToPage = false;
        sheet.PrintSetup.PaperSize = 9;//a4
        int colct = 9;//欄位數

        #region  資料
        //第一列-----------------------------
        sheet.CreateRow(0).CreateCell(0).SetCellValue(SheetName);
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, colct));//跨欄
        sheet.GetRow(0).GetCell(0).CellStyle = parm.cstitle;

        //資料列
        int i = 1;
        DataTable dt = new DataTable();
        foreach (DataRow item in dt.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i);
            sheet.GetRow(i).CreateCell(0).SetCellValue("xxx");
            sheet.GetRow(i).GetCell(0).CellStyle = parm.csdata;
        }
        #endregion

        //設定欄位寬度
        //sheet.SetColumnWidth(0, 13 * 256);
        //for (int x = 3; x < 9; x++)
        //    sheet.SetColumnWidth(x, 7 * 256);

        //sheet.SetColumnWidth(3, 8 * 256);

        sheet.DisplayGridlines = true;
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        result = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return result;
    }
    #endregion

    #region 合約院所執行表
    private byte[] Report4(ReportModel parm, string SheetName, HSSFWorkbook workbook)
    {
        byte[] result = null;
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(SheetName);
        sheet.FitToPage = false;
        sheet.PrintSetup.PaperSize = 9;//a4
        int colct = 9;//欄位數

        #region  資料
        //第一列-----------------------------
        sheet.CreateRow(0).CreateCell(0).SetCellValue(SheetName);
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, colct));//跨欄
        sheet.GetRow(0).GetCell(0).CellStyle = parm.cstitle;

        //資料列
        int i = 1;
        DataTable dt = new DataTable();
        foreach (DataRow item in dt.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(i);
            sheet.GetRow(i).CreateCell(0).SetCellValue("xxx");
            sheet.GetRow(i).GetCell(0).CellStyle = parm.csdata;
        }
        #endregion

        //設定欄位寬度
        //sheet.SetColumnWidth(0, 13 * 256);
        //for (int x = 3; x < 9; x++)
        //    sheet.SetColumnWidth(x, 7 * 256);

        //sheet.SetColumnWidth(3, 8 * 256);

        sheet.DisplayGridlines = true;
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        result = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return result;
    }
    #endregion

    #region 滿意度調查表
    private byte[] Report5(ReportModel parm, string SheetName, HSSFWorkbook workbook)
    {


        byte[] result = null;
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("sheet");//建立sheet
        sheet.FitToPage = false;//適合頁面
        sheet.PrintSetup.PaperSize = 9;//a4 紙張尺寸
        int colct = 9;//欄位數

        #region  固定資料
        //第一列-----------------------------
        sheet.CreateRow(0).CreateCell(0).SetCellValue(SheetName);//CreateRow新增列CreateCell新增欄SetCellValue加入值GetRow取得欄
        sheet.CreateRow(1).CreateCell(1).SetCellValue("裝置前收件日期");
        sheet.GetRow(1).CreateCell(2).SetCellValue("案件編號");
        sheet.GetRow(1).CreateCell(3).SetCellValue("個案姓名");
        sheet.GetRow(1).CreateCell(4).SetCellValue("性別");
        sheet.GetRow(1).CreateCell(5).SetCellValue("出生日期");
        sheet.GetRow(1).CreateCell(6).SetCellValue("身分證");
        sheet.GetRow(1).CreateCell(7).SetCellValue("電話");
        sheet.GetRow(1).CreateCell(8).SetCellValue("身分別");
        sheet.GetRow(1).CreateCell(9).SetCellValue("地址");
        sheet.GetRow(1).CreateCell(10).SetCellValue("合約院所");
        sheet.GetRow(1).CreateCell(11).SetCellValue("負責醫師");
        sheet.GetRow(1).CreateCell(12).SetCellValue("裝置項目");
        sheet.GetRow(1).CreateCell(13).SetCellValue("申請費用");
        sheet.GetRow(1).CreateCell(14).SetCellValue("裝置前審查委員會日期");
        sheet.GetRow(1).CreateCell(15).SetCellValue("裝置後審查委員會日期");
        sheet.GetRow(1).CreateCell(16).SetCellValue("從何知道");
        sheet.GetRow(1).CreateCell(17).SetCellValue("裝置後有無使用");
        sheet.GetRow(1).CreateCell(18).SetCellValue("裝置後是否舒適");
        sheet.GetRow(1).CreateCell(19).SetCellValue("承上題 不舒適原因");
        sheet.GetRow(1).CreateCell(20).SetCellValue("承上題 不舒適回診狀況");
        sheet.GetRow(1).CreateCell(21).SetCellValue("對縣長重要施政(假牙補助計畫)滿意嗎 ?");
        sheet.GetRow(1).CreateCell(22).SetCellValue("承上題 不滿意原因");
        sheet.GetRow(1).CreateCell(23).SetCellValue("對於假牙裝置補助是否有其他建議事項?");
        sheet.GetRow(1).CreateCell(24).SetCellValue("辦理情形");
        sheet.GetRow(1).CreateCell(25).SetCellValue("其他");//此欄空白
        sheet.GetRow(1).CreateCell(26).SetCellValue("關懷日期");//此欄空白
        sheet.GetRow(1).CreateCell(27).SetCellValue("問題改善結案");
        sheet.GetRow(1).CreateCell(28).SetCellValue("滿意度");


        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, colct));//跨欄(int 第幾列開始, int 合併幾列, int 第欄開始併, int 共欄位數(重A開始算))


        sheet.GetRow(0).GetCell(0).CellStyle = parm.cstitle;

        //資料列 (動態)
        List<AAAAAA> result1 = new List<AAAAAA>();
        using (dbEntities db = new dbEntities())
        {
            var Query = (from c in db.Comm_Case
                         join dt in db.Comm_Doctor on c.DeptSN equals dt.DeptSN
                         join s in db.Satisfaction on c.CaseNo equals s.CaseNo
                         join ci in db.Comm_CaseItem on c.CaseNo equals ci.CaseNo
                         join sb in db.Comm_Subsidy on ci.ItemSN equals sb.SN

                         where s.Status.Contains("結案") & ci.CaseNo == c.CaseNo
                         select new AAAAAA
                         {
                             BeforDate = c.BeforDate,
                             CaseNo = c.CaseNo,
                             Name = c.Name,
                             Gender = c.Gender,
                             Birthday = c.Birthday,
                             PID = c.PID,
                             Tel = c.Tel,
                             UserType = c.UserType,
                             Addr = c.Addr,
                             DtName = dt.Name,
                             CatregoryName = sb.Name,
                             WriteOffAmout = c.WriteOffAmout,
                             ProfDate = c.ProfDate,
                             WriteOffProDate = c.WriteOffProDate,
                             QuestionAns1 = s.QuestionAns1,
                             QuestionAns1Other = s.QuestionAns1Other,
                             QuestionAns2 = s.QuestionAns2,
                             QuestionAns3 = s.QuestionAns3,
                             QuestionAns4 = s.QuestionAns4,
                             QuestionAns5 = s.QuestionAns5,
                             QuestionAns6 = s.QuestionAns6,
                             QuestionAns7 = s.QuestionAns7,
                             Memo = s.Memo,
                             Remark = s.Remark,
                             QuestionAns5Otherfactor = s.QuestionAns5Otherfactor

                         });
            foreach (var data in Query)
            {
                result1.Add(data);
            }
        }
        int i = 2;
        int x = 1;


        for (int row = 0; row < result1.Count(); row++)
        {
            sheet.CreateRow(i).CreateCell(1).SetCellValue(string.Format("{0:yyy-MM-dd}", result1[row].BeforDate));
            sheet.GetRow(i).CreateCell(2).SetCellValue(result1[row].CaseNo);
            sheet.GetRow(i).CreateCell(3).SetCellValue(result1[row].Name);
            sheet.GetRow(i).CreateCell(4).SetCellValue(result1[row].Gender);//性別
            sheet.GetRow(i).CreateCell(5).SetCellValue(string.Format("{0:yyy-MM-dd}", result1[row].Birthday));//出生日
            sheet.GetRow(i).CreateCell(6).SetCellValue(result1[row].PID);
            sheet.GetRow(i).CreateCell(7).SetCellValue(result1[row].Tel);//電話
            sheet.GetRow(i).CreateCell(8).SetCellValue(result1[row].UserType);//身分別
            sheet.GetRow(i).CreateCell(9).SetCellValue(result1[row].Addr);//地址
            sheet.GetRow(i).CreateCell(10).SetCellValue(result1[row].DeptName);//合約院所
            sheet.GetRow(i).CreateCell(11).SetCellValue(result1[row].DtName);//醫師
            sheet.GetRow(i).CreateCell(12).SetCellValue(result1[row].CatregoryName);//申請項目
            sheet.GetRow(i).CreateCell(13).SetCellValue(Convert.ToDouble(result1[row].WriteOffAmout));//核銷-撥付費用
            sheet.GetRow(i).CreateCell(14).SetCellValue(string.Format("{0:yyy-MM-dd}", result1[row].ProfDate));//裝置前審查委員會日期
            sheet.GetRow(i).CreateCell(15).SetCellValue(string.Format("{0:yyy-MM-dd}", result1[row].WriteOffProDate));//裝置後審查委員會日期
            sheet.GetRow(i).CreateCell(16).SetCellValue(result1[row].QuestionAns1);//從何知道
            sheet.GetRow(i).CreateCell(17).SetCellValue(result1[row].QuestionAns1Other);//從何知道(其他)
            sheet.GetRow(i).CreateCell(18).SetCellValue(result1[row].QuestionAns2);//裝置後有無使用
            sheet.GetRow(i).CreateCell(19).SetCellValue(result1[row].QuestionAns3);//裝置後是否舒適
            sheet.GetRow(i).CreateCell(20).SetCellValue(result1[row].QuestionAns4);//承上題 不舒適原因
            sheet.GetRow(i).CreateCell(21).SetCellValue(result1[row].QuestionAns5);//承上題 不舒適回診狀況
            sheet.GetRow(i).CreateCell(22).SetCellValue(result1[row].QuestionAns6);//對縣長重要施政(假牙補助計畫)滿意嗎 ?
            sheet.GetRow(i).CreateCell(23).SetCellValue(result1[row].QuestionAns7);//承上題 不滿意原因
            sheet.GetRow(i).CreateCell(24).SetCellValue(result1[row].Memo);//對於假牙裝置補助是否有其他建議事項?
            sheet.GetRow(i).CreateCell(25).SetCellValue(result1[row].Remark);//辦理情形(備註)
            //sheet.GetRow(i).CreateCell(26).SetCellValue(result1[row].QuestionAns2);//其他 此欄空白
            //sheet.GetRow(i).CreateCell(27).SetCellValue(result1[row].QuestionAns2);//關懷日期 此欄空白
            sheet.GetRow(i).CreateCell(28).SetCellValue(result1[row].QuestionAns5Otherfactor);//問題改善結案
            //sheet.GetRow(i).CreateCell(29).SetCellValue(result1[row].QuestionAns2);//滿意度










            sheet.GetRow(i).CreateCell(6).SetCellValue(string.Format("{0:yyy-MM-dd}", result1[row].WriteOffProDate));
            i++;

        }

        #endregion

        //設定欄位寬度
        //sheet.SetColumnWidth(0, 13 * 256);
        //for (int x = 3; x < 9; x++)
        //    sheet.SetColumnWidth(x, 7 * 256);

        //sheet.SetColumnWidth(3, 8 * 256);

        sheet.DisplayGridlines = true;
        MemoryStream ms = new MemoryStream();

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        result = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return result;
    }
    #endregion
}


/// <summary>
/// 報表預設要使用的參數
/// </summary>
public class BaseReportModel
{
    public HSSFCellStyle cstitle { get; set; }
    /// <summary>
    /// 資料內容樣式
    /// </summary>
    public HSSFCellStyle csdata { get; set; }
}

/// <summary>
/// 報表要使用的參數
/// </summary>
public class ReportModel : BaseReportModel
{
    /// <summary>
    /// 收件日期-起
    /// </summary>
    public DateTime? Sdate { get; set; }
    /// <summary>
    /// 收件日期-迄
    /// </summary>
    public DateTime? Edate { get; set; }
    /// <summary>
    /// 會議類型
    /// </summary>
    public string MeetType { get; set; }
    /// <summary>
    /// 資料類型
    /// </summary>
    public string DataType { get; set; }

}


public class AAAAAA
{
    ///<summary>
    /// 案件編號，案件編號規則：民國年-月份-院所編碼-流水號*流水號，每月更新從001開始
    ///</summary>
    public string CaseNo { get; set; } // CaseNo (Primary key) (length: 15)

    ///<summary>
    /// 裝置前收件日期
    ///</summary>
    public System.DateTime? BeforDate { get; set; } // BeforDate

    ///<summary>
    /// 院所代號
    ///</summary>
    public string DeptSN { get; set; } // DeptSN (length: 3)

    ///<summary>
    /// 醫師代號
    ///</summary>
    public int DoctorSN { get; set; } // DoctorSN

    ///<summary>
    /// 身分證
    ///</summary>
    public string PID { get; set; } // PID (length: 50)

    ///<summary>
    /// 本縣設籍滿一年
    ///</summary>
    public bool? isLocalUser { get; set; } // isLocalUser

    ///<summary>
    /// 姓名
    ///</summary>
    public string Name { get; set; } // Name (length: 100)

    ///<summary>
    /// 身分別(一般、中低收入、身心障礙、原住民)
    ///</summary>
    public string UserType { get; set; } // UserType (length: 50)

    ///<summary>
    /// 生日
    ///</summary>
    public System.DateTime? Birthday { get; set; } // Birthday

    ///<summary>
    /// 電話
    ///</summary>
    public string Tel { get; set; } // Tel (length: 50)

    ///<summary>
    /// 性別(男、女)
    ///</summary>
    public string Gender { get; set; } // Gender (length: 2)

    ///<summary>
    /// 地址
    ///</summary>
    public string Addr { get; set; } // Addr (length: 255)

    ///<summary>
    /// 申請補助金額
    ///</summary>
    public int ApplyAmount { get; set; } // ApplyAmount

    ///<summary>
    /// 預定完成日期
    ///</summary>
    public System.DateTime? FinishDate { get; set; } // FinishDate

    ///<summary>
    /// 狀態(待審、行政審查通過、行政審查退件、專業審查通過、專業審查退件、核銷通過、核銷退件、特殊個案結案)
    ///</summary>
    public string Status { get; set; } // Status (length: 50)

    ///<summary>
    /// 建立者
    ///</summary>
    public string CreateID { get; set; } // CreateID (length: 50)

    ///<summary>
    /// 建立日期
    ///</summary>
    public System.DateTime CreateDate { get; set; } // CreateDate

    ///<summary>
    /// 修改人員帳號
    ///</summary>
    public string ModifyID { get; set; } // ModifyID (length: 50)

    ///<summary>
    /// 修改日期
    ///</summary>
    public System.DateTime ModifyDate { get; set; } // ModifyDate

    ///<summary>
    /// 刪除記號
    ///</summary>
    public bool isDelete { get; set; } // isDelete

    ///<summary>
    /// 行政審查日期
    ///</summary>
    public System.DateTime? AdminDate { get; set; } // AdminDate

    ///<summary>
    /// 行政審查結果->通過，退件
    ///</summary>
    public string AdminStatus { get; set; } // AdminStatus (length: 10)

    ///<summary>
    /// 行政審查退件原因
    ///</summary>
    public int? AdminRejectSN { get; set; } // AdminRejectSN

    ///<summary>
    /// 專業審查日期
    ///</summary>
    public System.DateTime? ProfDate { get; set; } // ProfDate

    ///<summary>
    /// 專業審查結果->通過，退件
    ///</summary>
    public string ProfStatus { get; set; } // ProfStatus (length: 10)

    ///<summary>
    /// 專業審查退件原因
    ///</summary>
    public int? ProfRejectSN { get; set; } // ProfRejectSN

    ///<summary>
    /// 專業審查委員，最多只能2個人，多筆逗號區隔，例如1,3
    ///</summary>
    public string ProfUser { get; set; } // ProfUser (length: 50)

    ///<summary>
    /// 核銷-裝置前審查通過發文日期
    ///</summary>
    public System.DateTime? WriteOffBefDate { get; set; } // WriteOffBefDate

    ///<summary>
    /// 核銷-裝置後收件日期
    ///</summary>
    public System.DateTime? WriteOffAftDate { get; set; } // WriteOffAftDate

    ///<summary>
    /// 核銷-專業審查日期
    ///</summary>
    public System.DateTime? WriteOffProDate { get; set; } // WriteOffProDate

    ///<summary>
    /// 核銷-撥付費用
    ///</summary>
    public int? WriteOffAmout { get; set; } // WriteOffAmout

    ///<summary>
    /// 核銷-專業審查委員，最多只能2個人，多筆逗號區隔，例如1,3
    ///</summary>
    public string WriteOffUser { get; set; } // WriteOffUser (length: 50)

    ///<summary>
    /// 核銷-是否有篩檢費->是、否
    ///</summary>
    public string WriteOffFee { get; set; } // WriteOffFee (length: 2)

    ///<summary>
    /// 核銷-專業審查結果-> 通過，退件/特殊結案結果->結案，特殊部份補助
    ///</summary>
    public string WriteOffStatus { get; set; } // WriteOffStatus (length: 20)

    ///<summary>
    /// 核銷-專業審查結果審查退件原因
    ///</summary>
    public int? WriteOffRejectSN { get; set; } // WriteOffRejectSN

    ///<summary>
    /// 核銷-是否移交社會局->是、否
    ///</summary>
    public string WriteOffTransfer { get; set; } // WriteOffTransfer (length: 2)

    ///<summary>
    /// 備註
    ///</summary>
    public string WriteOffMemo { get; set; } // WriteOffMemo (length: 255)

    ///<summary>
    /// 核銷類型->特殊個案結案，核銷專業審查
    ///</summary>
    public string WriteOffType { get; set; } // WriteOffType (length: 20)
    ///<summary>
    /// 院所名稱
    ///</summary>
    public string DeptName { get; set; } // DeptName (length: 100)

    ///<summary>
    /// 醫師姓名
    ///</summary>
    public string DtName { get; set; } // DeptName (length: 100)


    ///<summary>
    /// 項目名稱
    ///</summary>
    public string CatregoryName { get; set; } // DeptName (length: 100)
    ///<summary>
    /// 請問您從何處知道本項補助計畫
    ///</summary>
    public string QuestionAns1 { get; set; } // QuestionAns1 (length: 255)

    ///<summary>
    /// 其他說明
    ///</summary>
    public string QuestionAns1Other { get; set; } // QuestionAns1Other (length: 255)

    ///<summary>
    /// 請問目前您假牙裝置後有無使用？
    ///</summary>
    public string QuestionAns2 { get; set; } // QuestionAns2 (length: 50)

    ///<summary>
    /// 請問您對假牙裝置後是否舒適？
    ///</summary>
    public string QuestionAns3 { get; set; } // QuestionAns3 (length: 50)

    ///<summary>
    /// 承上題，請問您裝置後不舒適的原因是？
    ///</summary>
    public string QuestionAns4 { get; set; } // QuestionAns4 (length: 50)

    ///<summary>
    /// 承上題，目前您裝置後不舒適回診狀況？
    ///</summary>
    public string QuestionAns5 { get; set; } // QuestionAns5 (length: 50)

    ///<summary>
    /// 疾病因素無法前往
    ///</summary>
    public string QuestionAns5Otherfactor { get; set; } // QuestionAns5Otherfactor (length: 50)

    ///<summary>
    /// 您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？
    ///</summary>
    public string QuestionAns6 { get; set; } // QuestionAns6 (length: 50)

    ///<summary>
    /// 承上題，請問您不滿意的原因是？
    ///</summary>
    public string QuestionAns7 { get; set; } // QuestionAns7 (length: 50)

    ///<summary>
    /// 請問您對於假牙裝置補助是否有其他建議事項？
    ///</summary>
    public string Memo { get; set; } // Memo

    ///<summary>
    /// 不滿意後續追蹤
    ///</summary>
    public string DissatisfiedReason { get; set; } // DissatisfiedReason (length: 50)

    ///<summary>
    /// 個案轉衛生所追蹤
    ///</summary>
    public string HealthCenter { get; set; } // HealthCenter (length: 50)

    ///<summary>
    /// 備註
    ///</summary>
    public string Remark { get; set; } // Remark
}


