using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamastar.BusinessObject
{
    [Serializable]
    public partial class vw_SiteWebArchive : QueryBaseEntity<dbEntities, vw_SiteWebArchive>
    {
        #region 依條件查詢
        public static List<vw_SiteWebArchive> GetListData(string sortExpression, int maximumRows, int startRowIndex, int ParentNodeID, string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, sortExpression, ParentNodeID, KeyWord);
                var query = all.Skip(startRowIndex).Take(maximumRows);
                return query
                       .Select(a => a)
                       .AsNoTracking().ToList<vw_SiteWebArchive>();
            }
        }
        private static IQueryable<vw_SiteWebArchive> GetAllList(dbEntities db, string sortExpression, int ParentNodeID, string KeyWord, bool IsCount = false)
        {
            IQueryable<vw_SiteWebArchive> query;

            #region 處理排序
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.Contains(" DESC"))
                    query = DBHelper.OrderByDescending(db.vw_SiteWebArchive.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_SiteWebArchive>();
                else
                    query = DBHelper.OrderBy(db.vw_SiteWebArchive.Select(a => a), sortExpression.Replace(" DESC", "")).AsQueryable<vw_SiteWebArchive>();
            }
            else if (IsCount)
            {
                query = db.vw_SiteWebArchive
                 .Select(a => a);
            }
            else
            {
                query = db.vw_SiteWebArchive
                .OrderBy(a => a.Sort)
                .Select(a => a);
            }
            #endregion
            query = query.Where(a => a.IsDelete == false && a.ParentNodeID == ParentNodeID).Select(a => a);
            #region 查詢條件
            if (!string.IsNullOrEmpty(KeyWord))
            {
                query = query.Where(a => a.Name.Contains(KeyWord)).Select(a => a);
            }

            #endregion
            return query;
        }

        #endregion

        public static int GetListCount(int ParentNodeID, string KeyWord)
        {
            using (dbEntities db = new dbEntities())
            {
                var all = GetAllList(db, string.Empty, ParentNodeID, KeyWord, true).AsQueryable();
                var query = all;
                return query.AsNoTracking().Count();
            }
        }
    }

    [Serializable]
    public partial class vw_WebArchive : QueryBaseEntity<dbEntities, vw_WebArchive>
    {

    }

    [Serializable]
    public partial class Comm_WebArchive : BaseEntity<dbEntities, Comm_WebArchive>
    {

        /// <summary>
        /// 取得上層結點的名稱
        /// </summary>
        /// <param name="SN">The sn.</param>
        /// <returns></returns>
        public static string GetLvUpName(int NodeID)
        {
            using (dbEntities db = new dbEntities())
            {
                string Title = db.Comm_WebArchive
                            .Where(a => a.IsDelete == false && a.IsEnable == true && a.NodeID == NodeID)
                            .Select(x => x.Name).AsNoTracking().SingleOrDefault();
                return Title;
            }
        }
        /// <summary>
        /// 取得目前最大的排序
        /// </summary>
        /// <param name="ParentNodeID"></param>
        /// <returns></returns>
        public static int GetMaxSort(int ParentNodeID, int StitesSN)
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.ParentNodeID == ParentNodeID
                         && o.IsDelete == false && o.SitesSN == StitesSN
                        select o).Count();
            }
        }



        //取父層名稱
        public static string GetPartentName(int NodeID)
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        let detail = (from t2 in db.Comm_WebArchive
                                      where t2.NodeID == NodeID && t2.IsDelete == false && t2.IsEnable == true
                                      select t2.ParentNodeID).FirstOrDefault()
                        where detail == o.NodeID
                        select o.Name).FirstOrDefault();
            }
        }

        //取父類別資料
        public static List<Comm_WebArchive> GetArchiveList()
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.ParentNodeID == 0 && o.SitesSN == 1 && o.IsEnable == true && o.IsDelete == false
                        select o).ToList<Comm_WebArchive>();
            }
        }

        public static List<Comm_WebArchive> GetArchiveList_lv2()
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.ParentNodeID != 0 && o.SitesSN == 1 && o.IsEnable == true && o.IsDelete == false
                        select o).ToList<Comm_WebArchive>();
            }
        }

        public static List<Comm_WebArchive> GetArchiveListFront_lv1()
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.ParentNodeID == 0 && o.SitesSN == 2 && o.IsEnable == true && o.IsDelete == false
                        select o).ToList<Comm_WebArchive>();
            }
        }
        public static List<Comm_WebArchive> GetArchiveListFront_lv2(int SitesSN, int ParentNodeID)
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.ParentNodeID == ParentNodeID && o.SitesSN == SitesSN && o.IsEnable == true && o.IsDelete == false
                        select o).ToList<Comm_WebArchive>();
            }
        }

        //取子類別資料
        public static Comm_WebArchive GetArchive(int NodeID, int ParentNodeID)
        {
            using (dbEntities db = new dbEntities())
            {
                return (from o in db.Comm_WebArchive
                        where o.NodeID == NodeID && o.ParentNodeID == ParentNodeID && o.SitesSN == 1
                        select o).FirstOrDefault();
            }
        }


        //#region 依條件查詢
        //public static List<Comm_AccUser> GetListData( string KeyWord)
        //{
        //    using (dbEntities db = new dbEntities())
        //    {
        //        var Query = (from x in db.Comm_AccUser                             
        //                     select x);

        //        if (!string.IsNullOrEmpty(KeyWord))
        //            Query = Query.Where(x => x.ID.Contains(KeyWord));
        //        return Query.ToList();
        //    }
        //}
        //#endregion

        #region 取前台全部選單
        public static List<Comm_WebArchive> GetSitesWebArchiveByMenu()
        {
            using (dbEntities db = new dbEntities())
            {
                var datas = (from o in db.Comm_WebArchive
                             where o.SitesSN == 2
                             && o.IsOnMenu == true
                             && o.IsEnable == true
                             && o.IsDelete == false
                             orderby o.Sort ascending
                             select o).ToList<Comm_WebArchive>();
                return datas;
            }
        }

        public static List<Comm_WebArchive> GetSitesWebArchiveFat_FooterByMenu()
        {
            using (dbEntities db = new dbEntities())
            {
                var datas = (from o in db.Comm_WebArchive
                             where o.SitesSN == 2
                             && o.IsFooter == true
                             && o.IsEnable == true
                             && o.IsDelete == false
                             orderby o.Sort ascending
                             select o).ToList<Comm_WebArchive>();
                return datas;
            }
        }
        #endregion
    }
}
