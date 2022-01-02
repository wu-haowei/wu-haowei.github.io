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
    public partial class IdentityLog : BaseEntity<dbEntities, IdentityLog>
    {
        public static List<IdentityLog> GetData(int SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.IdentityLog
                             select x);
                return Query.ToList();
            }
        }

    }
}
