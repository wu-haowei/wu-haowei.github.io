using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
using Hamastar.Common;
using Hamastar.Common.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;

namespace Hamastar.BusinessObject
{
    [Serializable]
    public partial class vw_Comm_Case : BaseEntity<dbEntities, vw_Comm_Case>
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable]
    public partial class Comm_Case : BaseEntity<dbEntities, Comm_Case>
    {
        public static List<Comm_Case> GetListData(string sortExpression, int maximumRows, int startRowIndex, Dictionary<string, string> dicKeyWord, DateTime KeyWordBeforDateS, DateTime KeyWordBeforDateE)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, dicKeyWord, KeyWordBeforDateS, KeyWordBeforDateE);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<Comm_Case>();
            }
        }
        private static IQueryable<Comm_Case> GetAllList(dbEntities db, string sortExpression, Dictionary<string, string> dicKeyWord, DateTime KeyWordBeforDateS, DateTime KeyWordBeforDateE, bool IsCount = false)
        {
            IQueryable<Comm_Case> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_Case.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_Case>();
                else
                    query = DBHelper.OrderBy(db.Comm_Case.Select(a => a).OrderBy(a => a.Status).OrderBy(a => a.CaseNo), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_Case>();
            }
            else if (IsCount)
            {
                query = db.Comm_Case
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_Case
                .OrderBy(a => a.Status)
                .ThenBy(a => a.CaseNo)
                .Select(a => a);
            }
            #endregion
            //query = query.Select(a => a).Where(a => a.Status == 1);
            #region 查詢條件:key=欄位,value=查詢條件。ex:(身分證字號) "PID":"T123456789"

            //非日期的查詢
            foreach (var d in dicKeyWord)
            {
                if (!string.IsNullOrEmpty(d.Value))
                {
                    switch (d.Key)
                    {
                        case "PID": query = query.Where(a => a.PID.Equals(d.Value)); break;         //個案身分證字號
                        case "Name": query = query.Where(a => a.Name.Contains(d.Value)); break;     //個案姓名
                        case "Status": query = query.Where(a => a.Status == (d.Value)); break;      //案件狀態
                        case "DeptSN": query = query.Where(a => a.DeptSN.Contains(d.Value)); break; //院所代號(醫療院所名稱)
                        case "WriteOffTransfer": query = query.Where(a => a.WriteOffTransfer.Contains(d.Value)); break; //是否移送社會局
                    }
                }
            }

            if (null != KeyWordBeforDateS && null != KeyWordBeforDateE)
            {
                //裝置前收件日期(起迄) List[0]=起,[1]=迄
                query = query.Where(a => a.BeforDate >= KeyWordBeforDateS && a.BeforDate <= KeyWordBeforDateE);
            }

            #endregion

            return query;
        }

        public static int GetListCount(Dictionary<string, string> dicKeyWord, DateTime KeyWordBeforDateS, DateTime KeyWordBeforDateE)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, dicKeyWord, KeyWordBeforDateS, KeyWordBeforDateE, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }

        public static Comm_Case GetData(string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Case
                             where x.CaseNo == CaseNo
                             select x);
                return Query.FirstOrDefault();
            }
        }

        // 提供Lock鎖定的物件
        private static object thisLock = new object();
        /// <summary>
        /// 新增個案
        /// </summary>
        /// <param name="data">單頭</param>
        /// <param name="Detail">單身-診治項目</param>
        /// <param name="userID">useID</param>
        /// <returns></returns>
        public static Comm_Case InsertData(Comm_Case data, List<Comm_CaseItem> Detail, string userID)
        {
            lock (thisLock)
            {
                dbEntities db = new dbEntities();
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //單頭
                        int intNo = 0; //流水號
                        string year = (int.Parse(DateTime.Now.Year.ToString()) - 1911).ToString();
                        string month = DateTime.Now.Month.ToString();


                        //取當前資料庫案號最大流水號
                        var Query = from d in db.Comm_Case
                                    where d.DeptSN == data.DeptSN
                                    orderby d.CaseNo descending
                                    select d;
                        if (Query.Any())
                        {
                            //符合當下月份時，判斷流水號大於目前的暫存流水號(intNo)則更新intNo
                            foreach (var item in Query)
                            {
                                if (item.CaseNo.Substring(0, 3).Equals(year) && item.CaseNo.Substring(4, 2).Equals(month) && int.Parse(item.CaseNo.Substring(11, 3)) > intNo)
                                    intNo = int.Parse(item.CaseNo.Substring(11, 3));
                            }
                        }

                        string CaseNo = string.Format("{0}-{1}-{2}-{3:000}", year, month, data.DeptSN, (intNo + 1));
                        //string CaseNo = "110-10-012-001";
                        data.CaseNo = CaseNo;
                        data.Status = "待審";
                        data.isDelete = false;
                        db.Comm_Case.Add(data);
                        //detail
                        Detail.ForEach(x => x.CaseNo = CaseNo);
                        db.Comm_CaseItem.AddRange(Detail);
                        db.SaveChanges();

                        //記錄log
                        Comm_Record record = new Comm_Record();
                        record.NodeID = 13;
                        record.CaseNo = CaseNo;
                        record.ModifyAccountID = userID;
                        record.ModifyDate = DateTime.Now;
                        record.Action = "新增個案";
                        Comm_Record.Insert(record);

                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 更新個案
        /// </summary>
        /// <param name="data">單頭</param>
        /// <param name="Detail">單身-診治項目</param>
        /// <param name="Status">案件狀態</param>
        /// <param name="userID">useID</param>
        /// <returns></returns>
        public static Comm_Case updateData(Comm_Case data, List<Comm_CaseItem> Detail, string Status, string userID)
        {
            dbEntities db = new dbEntities();
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //單頭                        
                    data.Status = Status;
                    //data.ModifyID = "當下使用者";
                    //data.ModifyDate = DateTime.Now;
                    data.isDelete = false;
                    Comm_Case.Update(data);
                    //detail
                    var oldCaseItem = db.Comm_CaseItem.Where(a => a.CaseNo == data.CaseNo).Select(a => a);
                    List<Comm_CaseItem> listOldItem = new List<Comm_CaseItem>();
                    foreach (var item in oldCaseItem)
                    {
                        listOldItem.Add(item);
                    }
                    //移除舊的單身
                    db.Comm_CaseItem.RemoveRange(listOldItem);
                    db.SaveChanges();
                    Detail.ForEach(x => x.CaseNo = data.CaseNo);
                    //加入新的單身
                    db.Comm_CaseItem.AddRange(Detail);
                    db.SaveChanges();

                    //記錄log
                    Comm_Record record = new Comm_Record();
                    record.NodeID = 13;
                    record.CaseNo = data.CaseNo;
                    record.ModifyAccountID = userID;
                    record.ModifyDate = DateTime.Now;
                    record.Record = "編輯個案(審核前)";
                    Comm_Record.Insert(record);

                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }
            }
            return data;
        }

        public static string AuditPID(string PID)
        {
            string rtn = string.Empty;
            using (dbEntities db = new dbEntities())
            {
                //可申請條件1：案件狀態=特殊個案結案 and 撥付費用=0 and 篩檢費=是 and 裝置前專業審查日期>6M 
                var Query1 = (from x in db.Comm_Case
                              where x.PID.Equals(PID) && x.Status.Equals("特殊個案結案")
                              && x.WriteOffAmout <= 0 && x.WriteOffFee.Equals("是")
                              && DbFunctions.DiffDays(DbFunctions.AddMonths(x.ProfDate, 6), DateTime.Now) >= 0
                              select x).Any();
                //可申請條件2：曾申請過維修撥付金額<=6000
                var Query2 = (from x in db.Comm_Case
                              where x.PID.Equals(PID)
                              select x).Sum(s => s.WriteOffAmout);
                //可申請條件3：有診治項目申請紀錄者(若無則不可申請)
                //可申請條件4：有診治項目申請紀錄者，案件狀態 = 核銷審查通過 and 撥付費用 > 0 and 核銷專業審查日期 >= 1年 
                var Query3 = false;
                var Query4 = false;
                var QueryCase = (from x in db.Comm_Case select x).Where(x => x.PID.Equals(PID));
                bool HasItemType1 = false;
                if (QueryCase.Any())
                {
                    foreach (var data in QueryCase)
                    {
                        if ((from x in db.Comm_CaseItem where x.CaseNo.Equals(data.CaseNo) && x.ItemType == 1 select x).Any())
                            HasItemType1 = true;
                    }
                }
                if (HasItemType1)
                {
                    Query3 = true;
                    Query4 = (from x in db.Comm_Case
                              where x.PID.Equals(PID) && x.Status.Equals("核銷審查通過")
                              && x.WriteOffAmout > 0
                              && DbFunctions.DiffDays(DbFunctions.AddYears(x.WriteOffProDate, 1), DateTime.Now) >= 0
                              select x).Any();
                }

                //判斷條件是否符合，不符合則跳出提醒
                if (!Query1)
                    rtn = "此個案距離上次申請未滿6個月，暫不提供重新申請";
                else if (Query2 > 6000)
                    rtn = "此個案維修費補助申請已達到6000元上限";
                else if (!Query3)
                    rtn = "此個案並未有診治項目申請紀錄，請通知院所";
                else if (!Query4)
                    rtn = "此個案裝置未滿一年，暫不提供維護申請";
            }
            return rtn;
        }

    }

    #region 案件管理 view
    [Serializable]
    public partial class vw_Case : QueryBaseEntity<dbEntities, vw_Case>
    {
        #region 後台列表
        #region 依條件查詢
        public static List<vw_Case> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID, string KeyWordName, string KeyWordWriteOffTransfer, string CaseType)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWordBeforDateS, KeyWordBeforDateE, KeyWordStatus, KeyWordDeptSN, KeyWordPID, KeyWordName, KeyWordWriteOffTransfer, CaseType);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<vw_Case>();
            }
        }
        private static IQueryable<vw_Case> GetAllList(dbEntities db, string sortExpression, string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID, string KeyWordName, string KeyWordWriteOffTransfer, string CaseType, bool IsCount = false)
        {
            IQueryable<vw_Case> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Case.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Case>();
                else
                    query = DBHelper.OrderBy(db.vw_Case.Select(a => a).OrderBy(a => a.Status).OrderBy(a => a.CaseNo), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Case>();
            }
            else if (IsCount)
            {
                query = db.vw_Case
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Case
                .OrderBy(a => a.CaseNo)
                .Select(a => a);
            }
            #endregion
            //query = query.Select(a => a).Where(a => a.Status == 1);
            #region 查詢條件

            //依案件管理類型篩選
            if (!string.IsNullOrEmpty(CaseType))
            {
                switch (CaseType)
                {
                    case "CaseCL":  //已辦結案件
                        query = query.Where(a => a.Status.Equals("行政審查退件") || a.Status.Equals("核銷審查通過") || a.Status.Equals("特殊個案結案") || a.Status.Equals("特殊部分補助"));
                        break;
                    case "CaseRE":  //核銷案件
                        query = query.Where(a => a.Status.Equals("專業審查通過") || a.Status.Equals("核銷審查退件"));
                        break;
                    case "CasePR":  //初審案件
                        query = query.Where(a => a.Status.Equals("待審") || a.Status.Equals("行政審查通過") || a.Status.Equals("專業審查退件"));
                        break;
                    case "CaseOV":  //案件總覽
                    default:
                        break;
                }
            }


            if (!string.IsNullOrWhiteSpace(KeyWordBeforDateS) && DateTime.TryParse(KeyWordBeforDateS, out DateTime tryParseDatetimeS))
            {
                var year = KeyWordBeforDateS.Substring(0, 3);
                var month = KeyWordBeforDateS.Substring(4, 2);
                var day = KeyWordBeforDateS.Substring(7, 2);
                var BeforDate = DateTime.Parse($"{year}-{month}-{day}");
                query = query.Where(a => a.BeforDate >= BeforDate); //收件日期區間-起
            }

            if (!string.IsNullOrWhiteSpace(KeyWordBeforDateE) && DateTime.TryParse(KeyWordBeforDateE, out DateTime tryParseDatetimeE))
            {
                var year = KeyWordBeforDateE.Substring(0, 3);
                var month = KeyWordBeforDateE.Substring(4, 2);
                var day = KeyWordBeforDateE.Substring(7, 2);
                var BeforDate = DateTime.Parse($"{year}-{month}-{day}");
                query = query.Where(a => a.BeforDate <= BeforDate); //收件日期區間-迄
            }

            if (!string.IsNullOrWhiteSpace(KeyWordStatus))
                query = query.Where(a => a.Status.Equals(KeyWordStatus));   //案件狀態

            if (!string.IsNullOrWhiteSpace(KeyWordDeptSN))
                query = query.Where(a => a.DeptSN.Equals(KeyWordDeptSN));   //院所SN

            if (!string.IsNullOrWhiteSpace(KeyWordPID))
                query = query.Where(a => a.PID.Equals(KeyWordPID));         //個案身分證字號

            if (!string.IsNullOrWhiteSpace(KeyWordName))
                query = query.Where(a => a.Name.Contains(KeyWordName));         //個案姓名

            if (!string.IsNullOrWhiteSpace(KeyWordWriteOffTransfer))
                query = query.Where(a => a.WriteOffTransfer.Equals(KeyWordWriteOffTransfer));         //
            #endregion

            return query;
        }
        #endregion
        public static int GetListCount(string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID, string KeyWordName, string KeyWordWriteOffTransfer, string CaseType)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyWordBeforDateS, KeyWordBeforDateE, KeyWordStatus, KeyWordDeptSN, KeyWordPID, KeyWordName, KeyWordWriteOffTransfer, CaseType, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        #endregion

    }
    #endregion

}
