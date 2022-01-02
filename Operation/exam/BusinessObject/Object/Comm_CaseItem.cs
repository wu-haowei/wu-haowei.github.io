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
    public partial class Comm_CaseItem : BaseEntity<dbEntities, Comm_CaseItem>
    {
        public static List<Comm_CaseItem> GetListData(string sortExpression, int maximumRows, int startRowIndex, Dictionary<string, string> dicKeyWord, DateTime KeyWordBeforDateS, DateTime KeyWordBeforDateE)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, dicKeyWord, KeyWordBeforDateS, KeyWordBeforDateE);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<Comm_CaseItem>();
            }
        }
        private static IQueryable<Comm_CaseItem> GetAllList(dbEntities db, string sortExpression, Dictionary<string, string> dicKeyWord, DateTime KeyWordBeforDateS, DateTime KeyWordBeforDateE, bool IsCount = false)
        {
            IQueryable<Comm_CaseItem> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_CaseItem.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_CaseItem>();
                else
                    query = DBHelper.OrderBy(db.Comm_CaseItem.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_CaseItem>();
            }
            else if (IsCount)
            {
                query = db.Comm_CaseItem
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_CaseItem
                .Select(a => a);
            }
            #endregion
            //query = query.Select(a => a).Where(a => a.Status == 1);
            #region 查詢條件:key=欄位,value=查詢條件。ex:(身分證字號) "PID":"T123456789"

            //非日期的查詢
            //foreach (var d in dicKeyWord)
            //{
            //    if (!string.IsNullOrEmpty(d.Value))
            //    {
            //        switch (d.Key)
            //        {
            //            case "PID": query = query.Where(a => a.PID.Equals(d.Value)); break;         //個案身分證字號
            //            case "Name": query = query.Where(a => a.Name.Contains(d.Value)); break;     //個案姓名
            //            case "Status": query = query.Where(a => a.Status == (d.Value)); break;      //案件狀態
            //            case "DeptSN": query = query.Where(a => a.DeptSN.Contains(d.Value)); break; //院所代號(醫療院所名稱)
            //        }
            //    }
            //}

            //if (null != KeyWordBeforDateS && null != KeyWordBeforDateE)
            //{
            //    //裝置前收件日期(起迄) List[0]=起,[1]=迄
            //    query = query.Where(a => a.BeforDate >= KeyWordBeforDateS && a.BeforDate <= KeyWordBeforDateE);
            //}
            //if (WriteOffProDate.Count > 0)
            //{
            //    //裝置後審查會日期(起迄) List[0]=起,[1]=迄
            //    query = query.Where(a => a.WriteOffProDate >= WriteOffProDate[0] && a.WriteOffProDate <= WriteOffProDate[1]);
            //}

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

        public static (int? itemType, List<Comm_CaseItem> listItem) GetData(string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                List<Comm_CaseItem> listItem = new List<Comm_CaseItem>();
                var Query = (from x in db.Comm_CaseItem
                             where x.CaseNo == CaseNo
                             select x);
                foreach (var item in Query)
                {
                    listItem.Add(item);
                }
                if (Query.Any())
                    return (Query.First().ItemType, listItem);
                else
                    return (-1, null);
            }
        }

        //當下SN最大值
        public static int GetItemMaxSN()
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = from d in db.Comm_CaseItem orderby d.SN descending select d;
                if (Query.Any())
                    return Query.First().SN;
                else
                    return 0;
            }
        }

        //    // 提供Lock鎖定的物件
        //    private static object thisLock = new object();
        //    /// <summary>
        //    /// 新增個案
        //    /// </summary>
        //    /// <param name="data">單頭</param>
        //    /// <param name="Detail">單身-診治項目</param>
        //    /// <returns></returns>
        //    public static Comm_Case InsertData(Comm_Case data, List<Comm_CaseItem> Detail)
        //    {
        //        lock (thisLock)
        //        {
        //            dbEntities db = new dbEntities();
        //            using (var dbTransaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    //單頭
        //                    string strNo = ""; //流水號
        //                    int intNo = 0;

        //                    //取當前資料庫案號最大流水號
        //                    var Query = from d in db.Comm_Case
        //                                where d.DeptSN == data.DeptSN
        //                                orderby d.CaseNo descending
        //                                select d;
        //                    if (Query.Any())
        //                    {
        //                        var QueryCaseNo = Query.First().CaseNo;
        //                        strNo = QueryCaseNo.Substring(11, 3);
        //                        intNo = int.Parse(strNo);
        //                    }

        //                    string CaseNo = string.Format("{0}-{1}-{2}-{3:000}", (int.Parse(DateTime.Now.Year.ToString()) - 1911), DateTime.Now.Month, data.DeptSN, (intNo + 1));
        //                    //string CaseNo = "110-10-012-001";
        //                    data.CaseNo = CaseNo;
        //                    data.Status = "待審";
        //                    data.isDelete = false;
        //                    db.Comm_Case.Add(data);
        //                    //detail
        //                    Detail.ForEach(x => x.CaseNo = CaseNo);
        //                    db.Comm_CaseItem.AddRange(Detail);
        //                    db.SaveChanges();
        //                    dbTransaction.Commit();
        //                }
        //                catch
        //                {
        //                    dbTransaction.Rollback();
        //                }
        //            }
        //        }
        //        return data;
        //    }

        //    /// <summary>
        //    /// 更新個案
        //    /// </summary>
        //    /// <param name="data">單頭</param>
        //    /// <param name="Detail">單身-診治項目</param>
        //    /// <param name="Status">案件狀態</param>
        //    /// <returns></returns>
        //    public static Comm_Case updateData(Comm_Case data, List<Comm_CaseItem> Detail, string Status)
        //    {
        //        dbEntities db = new dbEntities();
        //        using (var dbTransaction = db.Database.BeginTransaction())
        //        {
        //            //try
        //            //{
        //            //單頭                        
        //            data.Status = Status;
        //            //data.ModifyID = "當下使用者";
        //            //data.ModifyDate = DateTime.Now;
        //            data.isDelete = false;
        //            Comm_Case.Update(data);
        //            //detail
        //            var oldCaseItem = db.Comm_CaseItem.Where(a => a.CaseNo == data.CaseNo).Select(a => a);
        //            List<Comm_CaseItem> listOldItem = new List<Comm_CaseItem>();
        //            foreach (var item in oldCaseItem)
        //            {
        //                listOldItem.Add(item);
        //            }
        //            //移除舊的單身
        //            db.Comm_CaseItem.RemoveRange(listOldItem);
        //            Detail.ForEach(x => x.CaseNo = data.CaseNo);
        //            //加入新的單身
        //            db.Comm_CaseItem.AddRange(Detail);
        //            db.SaveChanges();
        //            dbTransaction.Commit();
        //            //}
        //            //catch (Exception ex)
        //            //{
        //            //    dbTransaction.Rollback();
        //            //}
        //        }
        //        return data;
        //    }

        //}

        //#region 案件管理 view
        //[Serializable]
        //public partial class vw_Case : QueryBaseEntity<dbEntities, vw_Case>
        //{
        //    #region 後台列表
        //    #region 依條件查詢
        //    public static List<vw_Case> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID)
        //    {
        //        using (dbEntities db = new dbEntities())
        //        {
        //            var all = GetAllList(db, sortExpression, KeyWordBeforDateS, KeyWordBeforDateE, KeyWordStatus, KeyWordDeptSN, KeyWordPID);
        //            var query = all.Skip(startRowIndex).Take(maximumRows);
        //            return query
        //                   .Select(a => a)
        //                   .AsNoTracking().ToList<vw_Case>();
        //        }
        //    }
        //    private static IQueryable<vw_Case> GetAllList(dbEntities db, string sortExpression, string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID, bool IsCount = false)
        //    {
        //        IQueryable<vw_Case> query;

        //        #region 處理排序
        //        if (!string.IsNullOrEmpty(sortExpression))
        //        {
        //            if (sortExpression.Contains(" DESC"))
        //                query = DBHelper.OrderByDescending(db.vw_Case.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Case>();
        //            else
        //                query = DBHelper.OrderBy(db.vw_Case.Select(a => a).OrderBy(a => a.Status).OrderBy(a => a.CaseNo), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Case>();
        //        }
        //        else if (IsCount)
        //        {
        //            query = db.vw_Case
        //             .Select(a => a);
        //        }
        //        else
        //        {
        //            query = db.vw_Case
        //            .OrderBy(a => a.CaseNo)
        //            .Select(a => a);
        //        }
        //        #endregion
        //        //query = query.Select(a => a).Where(a => a.Status == 1);
        //        #region 查詢條件
        //        //eE, string , string , string 
        //        //if (!string.IsNullOrWhiteSpace(KeyWordBeforDateS) && DateTime.TryParse(KeyWordBeforDateS, out DateTime tryParseDatetimeS))
        //        //    query = query.Where(a => a.BeforDate >= DateTime.Parse(KeyWordBeforDateS)); //收件日期區間-起

        //        //if (!string.IsNullOrWhiteSpace(KeyWordBeforDateE) && DateTime.TryParse(KeyWordBeforDateE, out DateTime tryParseDatetimeE))
        //        //    query = query.Where(a => a.BeforDate <= DateTime.Parse(KeyWordBeforDateE)); //收件日期區間-迄

        //        if (!string.IsNullOrWhiteSpace(KeyWordStatus))
        //            query = query.Where(a => a.Status.Equals(KeyWordStatus));   //案件狀態

        //        if (!string.IsNullOrWhiteSpace(KeyWordDeptSN))
        //            query = query.Where(a => a.DeptSN.Equals(KeyWordDeptSN));   //院所SN

        //        if (!string.IsNullOrWhiteSpace(KeyWordPID))
        //            query = query.Where(a => a.PID.Equals(KeyWordPID));         //個案身分證字號
        //        #endregion

        //        return query;
        //    }
        //    #endregion
        //    public static int GetListCount(string KeyWordBeforDateS, string KeyWordBeforDateE, string KeyWordStatus, string KeyWordDeptSN, string KeyWordPID)
        //    {
        //        using (dbEntities db = new dbEntities())
        //        {
        //            var all = GetAllList(db, string.Empty, KeyWordBeforDateS, KeyWordBeforDateE, KeyWordStatus, KeyWordDeptSN, KeyWordPID, true).AsQueryable();
        //            var query = all;
        //            return query.AsNoTracking().Count();
        //        }
        //    }
        //    #endregion

        //}
        //#endregion
    }
}
