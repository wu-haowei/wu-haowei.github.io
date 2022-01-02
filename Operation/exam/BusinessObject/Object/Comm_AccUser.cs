using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Hamastar.BusinessObject
{
    [Serializable]
    public partial class vw_AccUser : QueryBaseEntity<dbEntities, vw_AccUser>
    {

        #region 後台列表
        #region 依條件查詢
        public static List<vw_AccUser> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWord);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<vw_AccUser>();
            }
        }
        private static IQueryable<vw_AccUser> GetAllList(dbEntities db, string sortExpression, string KeyWord, bool IsCount = false)
        {
            IQueryable<vw_AccUser> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_AccUser.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_AccUser>();
                else
                    query = DBHelper.OrderBy(db.vw_AccUser.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_AccUser>();
            }
            else if (IsCount)
            {
                query = db.vw_AccUser
                 .Select(a => a);
            }
            else
            {
                query = db.vw_AccUser
                .OrderBy(a => a.Status)
                .ThenBy(a => a.ID)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);
            #region 查詢條件
            if (KeyWord != null)
            {
                if (KeyWord != null)
                {
                    string strKeyword = KeyWord.ToLower();
                    query = query.Where(a => a.ID.ToLower().Contains(strKeyword));
                }

            }


            #endregion

            return query;
        }
        #endregion
        public static int GetListCount(string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyWord, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        #endregion
    }

    [Serializable]
    [MetadataType(typeof(Comm_AccUser))]

    public partial class Comm_AccUser : BaseEntity<dbEntities, Comm_AccUser>
    {
        //檢查帳號停用規則：1.逾半年未登入、 2.新增帳號後從逾半年未登入
        public static void CheckLastLogindateOverHalfYear()
        {
            using (dbEntities db = new dbEntities())
            {
                db.Database.ExecuteSqlCommand("exec CheckLastLogindateOverHalfYear");

            }

        }
        public static Comm_AccUser GetAccUser(string DeptID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_AccUser
                             where x.ID == DeptID
                             select x);
                return Query.FirstOrDefault();
            }
        }




    }
}
