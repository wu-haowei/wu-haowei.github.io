using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
namespace Hamastar.BusinessObject
{

    [Serializable]
    public partial class Comm_Department_IP : BaseEntity<dbEntities, Comm_Department_IP>
    {

        public static List<Comm_Department_IP> GetData(string DeptSN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Department_IP
                             where x.DeptSN == DeptSN
                             select x);
                return Query.ToList();
            }
        }

        public static List<Comm_Department_IP> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList();
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<Comm_Department_IP>();
            }
        }

        private static IQueryable<Comm_Department_IP> GetAllList()
        {
            IQueryable<Comm_Department_IP> query;
            using (dbEntities db = new dbEntities())
            {
                query = DBHelper.OrderByDescending(db.Comm_Department_IP.Select(a => a), "IP".Replace(" ASC", "")).AsQueryable<Comm_Department_IP>();

                // query = query.Select(a => a).Where(a => a.ParentId == "16");
            }
            return query;
        }
    }
}
