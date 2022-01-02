using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hamastar.BusinessObject
{
    [Serializable]
    public partial class Satisfaction : BaseEntity<dbEntities, Satisfaction>
    {

    }
    //改善GetInstallationcomplete()共用

    /// <summary>
    /// 裝置完成
    /// </summary>
    public partial class vw_Installationcomplete : BaseEntity<dbEntities, vw_Installationcomplete>
    {
        #region 滿意度管理
        public static List<vw_Installationcomplete> GetListData(string sortExpression, int maximumRows, int startRowIndex, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }
        private static IQueryable<vw_Installationcomplete> GetAllList(dbEntities db, string sortExpression, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName, bool IsCount = false)
        {
            IQueryable<vw_Installationcomplete> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Installationcomplete.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.vw_Installationcomplete.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.vw_Installationcomplete
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Installationcomplete
                .OrderBy(s => s.DeptSN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #endregion
            #region 查詢條件
            if (null != txtFromWriteOffProDate)
            {
                var EndWriteOffProDate = txtEndWriteOffProDate == null ? txtEndWriteOffProDate = DateTime.Now : txtEndWriteOffProDate.Value.AddDays(1);
                //裝置後審查會日期
                query = query.Where(a => a.WriteOffProDate >= txtFromWriteOffProDate && a.WriteOffProDate < EndWriteOffProDate);
            }
            if (null != DeptName)
            {
                //院所
                query = query.Where(a => a.DeptSN == DeptName);
            }
            #endregion
            return query;
        }
        public static int GetListCount(DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        public static Comm_Case GetCase(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Case
                             where x.CaseNo == ID
                             select x);
                return Query.FirstOrDefault();
            }
        }
        public static Satisfaction GetSatisfaction(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.Satisfaction.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
        public static vw_Installationcomplete GetInstallationcomplete(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.vw_Installationcomplete.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
    }
    /// <summary>
    /// 不滿意追蹤
    /// </summary>
    public partial class vw_Dissatisfied : BaseEntity<dbEntities, vw_Dissatisfied>
    {
        #region 滿意度管理
        public static List<vw_Dissatisfied> GetListData(string sortExpression, int maximumRows, int startRowIndex, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }
        private static IQueryable<vw_Dissatisfied> GetAllList(dbEntities db, string sortExpression, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName, bool IsCount = false)
        {
            IQueryable<vw_Dissatisfied> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Dissatisfied.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.vw_Dissatisfied.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.vw_Dissatisfied
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Dissatisfied
                .OrderBy(s => s.DeptSN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #endregion
            #region 查詢條件
            if (null != txtFromWriteOffProDate)
            {
                var EndWriteOffProDate = txtEndWriteOffProDate == null ? txtEndWriteOffProDate = DateTime.Now : txtEndWriteOffProDate.Value.AddDays(1);
                //裝置後審查會日期
                query = query.Where(a => a.WriteOffProDate >= txtFromWriteOffProDate && a.WriteOffProDate < EndWriteOffProDate);
            }
            if (null != DeptName)
            {
                //院所
                query = query.Where(a => a.DeptSN == DeptName);
            }
            #endregion
            return query;
        }
        public static int GetListCount(DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        public static Comm_Case GetCase(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Case
                             where x.CaseNo == ID
                             select x);
                return Query.FirstOrDefault();
            }
        }
        public static Satisfaction GetSatisfaction(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.Satisfaction.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
        public static vw_Dissatisfied GetInstallationcomplete(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.vw_Dissatisfied.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
    }
    /// <summary>
    /// 轉衛生所追蹤
    /// </summary>
    public partial class vw_Healthcenter : BaseEntity<dbEntities, vw_Healthcenter>
    {
        #region 滿意度管理
        public static List<vw_Healthcenter> GetListData(string sortExpression, int maximumRows, int startRowIndex, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }
        private static IQueryable<vw_Healthcenter> GetAllList(dbEntities db, string sortExpression, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName, bool IsCount = false)
        {
            IQueryable<vw_Healthcenter> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Healthcenter.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.vw_Healthcenter.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.vw_Healthcenter
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Healthcenter
                .OrderBy(s => s.DeptSN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #endregion
            #region 查詢條件
            if (null != txtFromWriteOffProDate)
            {
                var EndWriteOffProDate = txtEndWriteOffProDate == null ? txtEndWriteOffProDate = DateTime.Now : txtEndWriteOffProDate.Value.AddDays(1);
                //裝置後審查會日期
                query = query.Where(a => a.WriteOffProDate >= txtFromWriteOffProDate && a.WriteOffProDate < EndWriteOffProDate);
            }
            if (null != DeptName)
            {
                //院所
                query = query.Where(a => a.DeptSN == DeptName);
            }
            #endregion
            return query;
        }
        public static int GetListCount(DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        public static Comm_Case GetCase(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Case
                             where x.CaseNo == ID
                             select x);
                return Query.FirstOrDefault();
            }
        }
        public static Satisfaction GetSatisfaction(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.Satisfaction.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
        public static vw_Healthcenter GetInstallationcomplete(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.vw_Healthcenter.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
    }
    /// <summary>
    /// 歷史滿意度查詢
    /// </summary>
    public partial class vw_Caseclosed : BaseEntity<dbEntities, vw_Caseclosed>
    {
        #region 滿意度管理
        public static List<vw_Caseclosed> GetListData(string sortExpression, int maximumRows, int startRowIndex, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }
        private static IQueryable<vw_Caseclosed> GetAllList(dbEntities db, string sortExpression, DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName, bool IsCount = false)
        {
            IQueryable<vw_Caseclosed> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Caseclosed.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.vw_Caseclosed.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.vw_Caseclosed
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Caseclosed
                .OrderBy(s => s.DeptSN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #endregion
            #region 查詢條件
            if (null != txtFromWriteOffProDate)
            {
                var EndWriteOffProDate = txtEndWriteOffProDate == null ? txtEndWriteOffProDate = DateTime.Now : txtEndWriteOffProDate.Value.AddDays(1);
                //裝置後審查會日期
                query = query.Where(a => a.WriteOffProDate >= txtFromWriteOffProDate && a.WriteOffProDate < EndWriteOffProDate);
            }
            if (null != DeptName)
            {
                //院所
                query = query.Where(a => a.DeptSN == DeptName);
            }
            #endregion
            return query;
        }
        public static int GetListCount(DateTime? txtFromWriteOffProDate, DateTime? txtEndWriteOffProDate, string DeptName)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, txtFromWriteOffProDate, txtEndWriteOffProDate, DeptName, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        public static Comm_Case GetCase(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Case
                             where x.CaseNo == ID
                             select x);
                return Query.FirstOrDefault();
            }
        }
        public static Satisfaction GetSatisfaction(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.Satisfaction.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
        public static vw_Caseclosed GetInstallationcomplete(string ID)
        {
            using (dbEntities db = new dbEntities())
            {
                return db.vw_Caseclosed.Where(w => w.CaseNo == ID).FirstOrDefault();

            }

        }
    }
}
