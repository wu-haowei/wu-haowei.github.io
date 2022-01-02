using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Hamastar.BusinessObject
{

    [Serializable]
    public partial class vw_Department : QueryBaseEntity<dbEntities, vw_Department>
    {
        #region 後台列表
        #region 依條件查詢
        public static List<vw_Department> GetListData(string sortExpression, int maximumRows, int startRowIndex, string KeyWorddName, string KeyWordSN, string KeyWordAreaSN)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, KeyWorddName, KeyWordSN, KeyWordAreaSN);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<vw_Department>();
            }
        }
        private static IQueryable<vw_Department> GetAllList(dbEntities db, string sortExpression, string KeyWorddName, string KeyWordSN, string KeyWordAreaSN, bool IsCount = false)
        {
            IQueryable<vw_Department> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_Department.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Department>();
                else
                    query = DBHelper.OrderBy(db.vw_Department.Select(a => a).OrderBy(a => a.Status).OrderBy(a => a.SN), sortExpression.Replace(" DESC", "")).AsQueryable<vw_Department>();
            }
            else if (IsCount)
            {
                query = db.vw_Department
                 .Select(a => a);
            }
            else
            {
                query = db.vw_Department
                .OrderBy(a => a.Status)
                .ThenBy(a => a.SN)
                .Select(a => a);
            }
            #endregion
            //query = query.Select(a => a).Where(a => a.Status == 1);
            #region 查詢條件


            if (!string.IsNullOrEmpty(KeyWorddName))
            {
                //院所名稱
                query = query.Where(a => a.DeptName.Contains(KeyWorddName));
            }
            if (!string.IsNullOrEmpty(KeyWordSN))
            {
                //院所編號
                query = query.Where(a => a.SN == KeyWordSN);
            }
            if (!string.IsNullOrEmpty(KeyWordAreaSN))
            {
                //鄉鎮巿區
                query = query.Where(a => a.AreaID.Equals(KeyWordAreaSN));
            }

            #endregion

            return query;
        }
        #endregion
        public static int GetListCount(string KeyWorddName, string KeyWordSN, string KeyWordAreaSN)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, KeyWorddName, KeyWordSN, KeyWordAreaSN, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
        #endregion

    }

    [Serializable]
    [MetadataType(typeof(Comm_Department))]

    public partial class Comm_Department : BaseEntity<dbEntities, Comm_Department>
    {
        /// <summary>
        /// 取得最大號
        /// </summary>
        /// <returns></returns>
        private static string GetDepartmentMaxNo(dbEntities db)
        {
            int intSN = -1;
            string SN = "";
            var data = (from x in db.Comm_Department select x.SN).Max();
            if (string.IsNullOrEmpty(data))
                SN = "001";
            else
            {
                bool isSN = int.TryParse(data, out intSN);
                if (isSN)
                    SN = string.Format("{0:000}", (intSN + 1));
            }
            return SN;
        }


        #region 新增資料(包含案號)
        // 提供Lock鎖定的物件
        private static object thisLock = new object();

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <returns></returns>
        public static Comm_Department InsertData(Comm_Department data, List<Comm_Department_IP> Detail)
        {
            lock (thisLock)
            {
                dbEntities db = new dbEntities();
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //單頭
                        data.SN = GetDepartmentMaxNo(db);
                        db.Comm_Department.Add(data);
                        //detail
                        Detail.ForEach(x => x.DeptSN = data.SN);
                        Detail.ForEach(x => x.CreateDepartmentID = data.CreateDepartmentID);
                        Detail.ForEach(x => x.CreateAccountID = data.CreateAccountID);
                        Detail.ForEach(x => x.CreateDate = DateTime.Now);
                        db.Comm_Department_IP.AddRange(Detail);
                        db.SaveChanges();
                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        //    ans = ex.ToString();
                    }
                }
            }
            return data;
        }
        #endregion

        #region 修改資料
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <returns></returns>
        public static Comm_Department UpdateData(Comm_Department data, List<Comm_Department_IP> Detail, string SN)
        {
            dbEntities db = new dbEntities();
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //單頭
                    data.SN = SN;
                    Comm_Department.Update(data);
                    //detail
                    #region 刪除舊的
                    var oldData = from d in db.Comm_Department_IP where d.DeptSN.Equals(SN) select d;
                    foreach (var d in oldData)
                    {
                        db.Comm_Department_IP.Remove(d);
                    }
                    #endregion
                    Detail.ForEach(x => x.DeptSN = data.SN);
                    Detail.ForEach(x => x.CreateDepartmentID = data.CreateDepartmentID);
                    Detail.ForEach(x => x.CreateAccountID = data.CreateAccountID);
                    Detail.ForEach(x => x.CreateDate = DateTime.Now);
                    db.Comm_Department_IP.AddRange(Detail);
                    db.SaveChanges();
                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    //    ans = ex.ToString();
                }
            }
            return data;
        }
        #endregion

        public static Comm_Department GetDepartment(string SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Department
                             where x.SN == SN
                             select x);
                return Query.FirstOrDefault();
            }
        }

        public static (string str, bool bl) IsIP(string IP)
        {
            string rtnIP = "";
            bool rtn = false;   //是否為合法IP
            IPAddress ip;
            Uri url;    //若IP有加port的時候，只是IPAddress.TryParse會無法解析，所以加上url
            if (Uri.TryCreate(string.Format("http://{0}", IP), UriKind.Absolute, out url))
            {
                if (IPAddress.TryParse(url.Host, out ip))
                {
                    rtnIP = ip.ToString();
                    rtn = true;
                }
                else
                {
                    //當IP是IPV6時，使用Uri.CheckHostName
                    var chkHostInfo = Uri.CheckHostName(IP);
                    if (chkHostInfo == UriHostNameType.IPv6)
                    {
                        if (IPAddress.TryParse(IP, out ip))
                        {
                            rtn = true;
                            rtnIP = ip.ToString();
                        }
                        else
                        {
                            //解析port
                            int colonPos = IP.LastIndexOf(":");
                            if (colonPos > 0)
                            {
                                string tempIp = IP.Substring(0, colonPos - 1);
                                if (IPAddress.TryParse(tempIp, out ip))
                                {
                                    rtnIP = ip.ToString();
                                    rtn = true;
                                }
                            }
                        }
                    }
                }
            }
            return (rtnIP, rtn);
        }

        /// <summary>
        /// 取得所有院所(狀態為啟用中):醫師資料用
        /// </summary>
        /// <returns></returns>
        public static List<Comm_Department> GetAllDepartmentNotStop()
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Department
                             where x.Status == 1
                             select x);
                return Query.ToList();
            }
        }
        /// <summary>
        /// 取得所有院所(滿意度管理)
        /// </summary>
        /// <returns></returns>
        public static List<Comm_Department> GetAllDepartment()
        {
            using (dbEntities db = new dbEntities())
            {
                var Query = (from x in db.Comm_Department
                             .Where(w => w.Status == 1)//狀態:1啟用、2停用 
                             select x);
                return Query.ToList();
            }
        }
    }
}
