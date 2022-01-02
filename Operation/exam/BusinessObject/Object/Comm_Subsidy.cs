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
    public partial class Comm_Subsidy : BaseEntity<dbEntities, Comm_Subsidy>
    {
        public static List<Comm_Subsidy> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWord, string Category)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWord, Category);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query.Select(a => a).ToList();
            }
        }

        public static Comm_Subsidy GetData(int SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Subsidy
                             where x.SN == SN
                             select x);
                return Query.FirstOrDefault();
            }
        }
        private static IEnumerable<Comm_Subsidy> GetAllList(dbEntities db, string sortExpression, string KeyWord, string Category, bool IsCount = false)
        {
            IQueryable<Comm_Subsidy> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.Comm_Subsidy.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
                else
                    query = DBHelper.OrderBy(db.Comm_Subsidy.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable();
            }
            else if (IsCount)
            {
                query = db.Comm_Subsidy
                 .Select(a => a);
            }
            else
            {
                query = db.Comm_Subsidy
               .OrderBy(s => s.Status).ThenBy(c => c.Catregory).ThenBy(s => s.SN)
                .Select(a => a);
            }
            #endregion
            query = query.Select(a => a);


            #region 查詢條件
            if (!string.IsNullOrEmpty(KeyWord))
            {
                var input = KeyWord.Trim();//清除首尾空白
                query = query.Where(a => a.Name.Contains(input));
            }
            if (!string.IsNullOrEmpty(Category))
            {
                int SelectedItem = Convert.ToInt32(Category);
                query = query.Where(a => a.Catregory == SelectedItem);
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

        public static Comm_Subsidy GetSubsidy(int DeptID)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Subsidy
                             where x.SN == DeptID
                             select x);
                return Query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 所有補助項目
        /// </summary>
        /// <returns></returns>
        public static List<Comm_Subsidy> GetAllSubsidy()
        {
            List<Comm_Subsidy> rtn = new List<Comm_Subsidy>();
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Subsidy
                             where x.Status == 1
                             select x);
                if (Query.Any())
                {
                    foreach (var d in Query)
                    {
                        rtn.Add(d);
                    }
                }
            }
            return rtn;
        }

        /// <summary>
        /// 被選取的補助項目金額總計
        /// </summary>
        /// <param name="listComm_Subsidy">新增個案時符合的補助項目</param>
        /// <param name="dicSelectItem">選取的項目及其對應數量</param>
        /// <param name="Catregory">項目類型:1=診治項目、2=維修費項目</param>
        /// <returns></returns>
        public static int SumAmountIsSelected(List<Comm_Subsidy> listComm_Subsidy, Dictionary<int, int> dicSelectItem, int Catregory)
        {
            int rtn = 0;
            var dataList = from data in listComm_Subsidy
                           join data2 in dicSelectItem on data.SN equals data2.Key
                           where data.Catregory == Catregory
                           select data.Amount * data2.Value;
            foreach (int sum in dataList)
            {
                //將篩選出符合有勾選的金額加總
                rtn += sum;
            }

            return rtn;
        }

    }
}
