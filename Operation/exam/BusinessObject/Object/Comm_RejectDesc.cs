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
    public partial class Comm_RejectDesc : BaseEntity<dbEntities, Comm_RejectDesc>
    {
        public static List<Comm_RejectDesc> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord, string Category)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWord, Category);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }
        public static Comm_RejectDesc GetData(int? SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_RejectDesc
                             where x.SN == SN
                             select x);
                return Query.FirstOrDefault();
            }
        }

        private static IEnumerable<Comm_RejectDesc> GetAllList(dbEntities db, string sortExpression, string KeyWord, string Category, bool IsCount = false)
        {
            IQueryable<Comm_RejectDesc> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_RejectDesc.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.Comm_RejectDesc.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.Comm_RejectDesc
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_RejectDesc
                .OrderBy(s => s.Status).ThenBy(c => c.Catregory).ThenBy(s => s.SN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);


            #region 查詢條件
            if (!string.IsNullOrEmpty(KeyWord))
            {
                var input = KeyWord.Trim();//清除首尾空白
                query = query.Where(a => a.RejectDesc.Contains(input));
            }
            if (!string.IsNullOrEmpty(Category))
            {
                int ca = Convert.ToInt32(Category);
                query = query.Where(a => a.Catregory == ca);
            }

            #endregion

            return query;
        }
        public static int GetListCount(string KeyWord, string Category)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyWord, Category, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }

        public static Comm_RejectDesc GetRejectDesc(int DeptID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_RejectDesc
                             where x.SN == DeptID
                             select x);
                return Query.FirstOrDefault();
            }
        }

        #region 案件管理
        public static List<Comm_RejectDesc> GetRejectDescForCase(int Catregory)
        {
            using (dbEntities db = new dbEntities())
            {
                List<Comm_RejectDesc> listRejectDesc = new List<Comm_RejectDesc>();
                var Query = (from x in db.Comm_RejectDesc
                             where x.Catregory == Catregory && x.Status == 1
                             select x);
                foreach (var data in Query)
                {
                    listRejectDesc.Add(data);
                }
                return listRejectDesc;
            }
        }
        #endregion
    }
}
