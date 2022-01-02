using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
using Hamastar.Common;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace Hamastar.BusinessObject
{


    [Serializable]
    public partial class Comm_Area : BaseEntity<dbEntities, Comm_Area>
    {
        /// <summary>
        /// 取得所有鄉鎮巿
        /// </summary>
        /// <returns></returns>
        public static List<Comm_Area> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList();
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<Comm_Area>();
            }
        }

        private static IQueryable<Comm_Area> GetAllList()
        {
            IQueryable<Comm_Area> query;
            using (dbEntities db = new dbEntities())
            {
                query = DBHelper.OrderByDescending(db.Comm_Area.Select(a => a), "ID".Replace(" ASC", "")).AsQueryable<Comm_Area>();
                query = query.Select(a => a).Where(a => a.ParentId == "16");
            }
            return query;
        }

        /// <summary>
        /// 取得所有鄉鎮巿
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static List<Comm_Area> GetArea(string ParentID)
        {
            List<Comm_Area> rtnQuery = new List<Comm_Area>();
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Area
                             where x.ParentId == ParentID
                             select x);

                foreach(var d in Query)
                {
                    rtnQuery.Add(d);
                }

                return rtnQuery;
            }
        }

        /// <summary>
        /// 取得鄉鎮巿郵遞區號
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static string GetAreaPostID(string ID)
        {
            string rtn = "";
            using (dbEntities db = new dbEntities())
            {
                rtn = (from x in db.Comm_Area
                       where x.Id.Equals(ID)
                       select x.PostId).FirstOrDefault();
            }
            return rtn;
        }
    }
}
