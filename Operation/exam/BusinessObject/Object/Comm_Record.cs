using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;

namespace Hamastar.BusinessObject
{

    [Serializable]
    public partial class vw_Record : QueryBaseEntity<dbEntities, vw_Record>
    {
        public static List<vw_Record> GetvwRecord()
        {
            using (dbEntities db = new dbEntities())
            {
                return (from x in db.vw_Record
                        select x).ToList<vw_Record>();
            }
        }

        //個案管理
        public static List<vw_Record> GetvwRecordForCase(int NodeID, string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.vw_Record where x.CaseNo.Equals(CaseNo) select x);
                if (NodeID == 14)   //已辦結案件
                    Query = Query.Where(w => w.NodeID >= 12 && w.NodeID <= 14);
                else
                    Query = Query.Where(w => w.NodeID == NodeID);

                return Query.OrderBy(o => o.ModifyDate).ToList<vw_Record>();
            }
        }

        public static string DistinctvwRecord()
        {
            using (dbEntities db = new dbEntities())
            {
                var arraystr = (from cq in db.vw_Record
                                where cq.NodeName != null
                                select cq.NodeName).Distinct().ToList();

                if (arraystr == null)
                    return "";
                else
                {
                    string name = string.Join("、", arraystr.Select(x => x).ToArray());
                    return name;
                }
            }
        }
        //匯出excel"帳號", "姓名", "修改日期時間", "上線位置", "操作系統", "操作內容" 
        public static DataTable GetExcelDataList(string sortExpression, string isall, int s, int n, string txtID, string txtName, string ddlNodeName, string SDate, string EDate, string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, isall, s, n, txtID, txtName, ddlNodeName, SDate, EDate, CaseNo);
                if (all.Any())
                {
                    return (from a in all
                            join b in db.Comm_AccUser on a.ModifyAccountID equals b.ID
                            select new
                            {
                                帳號 = a.ModifyAccountID,
                                姓名 = b.Name,
                                修改日期時間 = a.ModifyDate,
                                上線位置 = a.IP,
                                操作系統 = a.NodeName,
                                操作內容 = a.Record.Replace("@@", "\n").Replace("<br/>", "\n")
                            }).ConvertDataTable();

                }
                else
                    return new DataTable();
            }
        }


        #region 後台列表
        #region 依條件查詢
        public static List<vw_Record> GetListData(string sortExpression, int maximumRows, int startRowIndex, string isall, int s, int n, string txtID, string txtName, string ddlNodeName, string SDate, string EDate, string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, isall, s, n, txtID, txtName, ddlNodeName, SDate, EDate, CaseNo);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                        .Select(a => a)
                       .AsNoTracking().ToList<vw_Record>();
            }
        }
        private static IQueryable<vw_Record> GetAllList(dbEntities db, string sortExpression, string isall, int s, int n, string txtID, string txtName, string ddlNodeName, string SDate, string EDate, string CaseNo, bool IsCount = false)
        {
            IQueryable<vw_Record> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Record.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Record>();
                else
                    query = DBHelper.OrderBy(db.vw_Record.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Record>();
            }
            else if (IsCount)
            {
                query = db.vw_Record
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Record
                .OrderByDescending(a => a.ModifyDate)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #region 查詢條件
            if (!string.IsNullOrWhiteSpace(txtID))  //帳號
            {
                string strtxtID = txtID.ToLower();
                query = query.Where(a => a.ModifyAccountID.ToLower().Contains(strtxtID));
            }
            if (!string.IsNullOrWhiteSpace(txtName))  //姓名
            {
                string strtxtName = txtName.ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(strtxtName));
            }
            if (!string.IsNullOrWhiteSpace(CaseNo))//案號
                query = query.Where(x => x.CaseNo == CaseNo);

            if (ddlNodeName != "0")  //單元名稱
            {
                string strddlNodeName = ddlNodeName.ToLower();
                query = query.Where(a => a.NodeName.ToLower().Contains(strddlNodeName));
            }
            if (!string.IsNullOrEmpty(SDate))  //起始時間
            {
                DateTime? dt = DateTime.Parse(SDate);
                query = query.Where(a => a.ModifyDate >= dt);
            }
            if (!string.IsNullOrEmpty(EDate)) //結束時間
            {
                DateTime? dt = DateTime.Parse(Convert.ToDateTime(EDate).AddDays(1).ToString());
                query = query.Where(a => a.ModifyDate < dt);
            }


            if (isall == "N")
            {
                if (s != 0)
                {
                    int DataSN = s;
                    query = query.Where(a => a.DataSN == DataSN);
                }

                if (n != 0)
                {
                    int NodeID = n;
                    query = query.Where(a => a.NodeID == NodeID);
                }
            }
            #endregion

            return query;
        }
        #endregion
        public static int GetListCount(string isall, int s, int n, string txtID, string txtName, string ddlNodeName, string SDate, string EDate, string CaseNo)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, isall, s, n, txtID, txtName, ddlNodeName, SDate, EDate, CaseNo, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        #endregion

    }

    [Serializable]
    public partial class Comm_Record : BaseEntity<dbEntities, Comm_Record>
    {
        //#region 依條件查詢
        //public static List<Comm_AccUser> GetListData( string KeyWord)
        //{
        //    using (dbEntities db = new dbEntities())
        //    {
        //        var Query = (from x in db.Comm_AccUser                             
        //                     select x);

        //        if (!string.IsNullOrEmpty(KeyWord))
        //            Query = Query.Where(x => x.ID.Contains(KeyWord));
        //        return Query.ToList();
        //    }
        //}
        //#endregion
    }
}
