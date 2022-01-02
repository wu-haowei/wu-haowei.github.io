using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
namespace Hamastar.BusinessObject
{

    [Serializable]
    public partial class Comm_Doctor : BaseEntity<dbEntities, Comm_Doctor>
    {

        public static List<Comm_Doctor> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyDeptSN, string KeyName, string KeyStatus)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyDeptSN, KeyName, KeyStatus);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<Comm_Doctor>();
            }
        }
        private static IQueryable<Comm_Doctor> GetAllList(dbEntities db, string sortExpression, string KeyDeptSN, string KeyName, string KeyStatus, bool IsCount = false)
        {
            IQueryable<Comm_Doctor> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_Doctor.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_Doctor>();
                else
                    query = DBHelper.OrderBy(db.Comm_Doctor.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<Comm_Doctor>();
            }
            else if (IsCount)
            {
                query = db.Comm_Doctor
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_Doctor
                .OrderBy(a => a.Status)
                .ThenBy(a => a.DeptSN)
                .ThenBy(a => a.Name)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #region 查詢條件  
            if (!string.IsNullOrEmpty(KeyDeptSN))
            {
                //服務院所
                query = query.Where(a => a.DeptSN.Equals(KeyDeptSN));
            }
            if (!string.IsNullOrEmpty(KeyName))
            {
                //醫師姓名
                query = query.Where(a => a.Name.Contains(KeyName));
            }
            if (!string.IsNullOrEmpty(KeyStatus))
            {
                //啟用狀態
                int Status = 0;
                bool hasStatus = int.TryParse(KeyStatus, out Status);
                if (hasStatus)
                    query = query.Where(a => a.Status == Status);
            }
            #endregion
            return query;
        }

        public static int GetListCount(string KeyDeptSN, string KeyName, string KeyStatus)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyDeptSN, KeyName, KeyStatus, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }

        /// <summary>
        /// 取得醫師資料
        /// </summary>
        /// <param name="SN">醫師編號</param>
        /// <returns></returns>
        public static Comm_Doctor GetData(int SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Doctor
                             where x.SN == SN
                             select x);
                return Query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 取得所有醫師資料
        /// </summary>
        /// <param name="DeptSN">院所代碼</param>
        /// <returns></returns>
        public static List<Comm_Doctor> GetAllData(string DeptSN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Doctor
                             where x.DeptSN == DeptSN
                             && x.Status == 1
                             select x);
                return Query.ToList();
            }
        }

        /// <summary>
        /// 醫師姓名是否重複
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool DoctorIsRepeat(string Name)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Doctor
                             where x.Name.Equals(Name)
                             select x).Any();
                return Query;
            }
        }
    }
}
