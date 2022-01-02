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
    public partial class Comm_Examiner : BaseEntity<dbEntities, Comm_Examiner>
    {
        /// <summary>
        /// 資料集合
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="maximumRows"></param>
        /// <param name="startRowIndex">跳過數量</param>
        /// <param name="KeyWord">查詢</param>
        /// <param name="KeyWord">狀態</param>
        /// <returns></returns>
        public static List<Comm_Examiner> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord, string KeyStatus)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWord, KeyStatus);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();

            }
        }

        public static Comm_Examiner GetData(int SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Examiner
                             where x.SN == SN
                             select x);
                return Query.FirstOrDefault();
            }
        }
        private static IEnumerable<Comm_Examiner> GetAllList(dbEntities db, string sortExpression, string KeyWord, string KeyStatus, bool IsCount = false)
        {
            IQueryable<Comm_Examiner> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_Examiner.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.Comm_Examiner.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.Comm_Examiner
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_Examiner
                  .OrderBy(s => s.Status).ThenBy(s => s.SN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #region 查詢條件

            if (!string.IsNullOrEmpty(KeyWord))
            {
                var input = KeyWord.Trim();
                query = query.Where(a => a.Name.Contains(input));
            }
            if (!string.IsNullOrEmpty(KeyStatus))
            {
                int St = Convert.ToInt32(KeyStatus);
                query = query.Where(a => a.Status == St);
            }

            #endregion

            return query;
        }
        public static int GetListCount(string KeyWord, string KeyStatus)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyWord, KeyStatus, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }

        public static Comm_Examiner GetExaminer(int DeptID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Examiner
                             where x.SN == DeptID
                             select x);
                return Query.FirstOrDefault();
            }
        }

        #region 個案管理
        public static List<Comm_Examiner> GetAllData()
        {
            List<Comm_Examiner> listExaminer = new List<Comm_Examiner>();
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Examiner
                             where x.Status == 1
                             select x);
                if (Query.Any())
                {
                    foreach (var data in Query)
                    {
                        listExaminer.Add(data);
                    }
                }

                return listExaminer;
            }
        }
        #endregion

    }
}
