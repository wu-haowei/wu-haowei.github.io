using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamastar.BusinessObject
{
    [Serializable]
    public partial class Comm_RelFile : BaseEntity<dbEntities, Comm_RelFile>
    {


        /// <summary>
        /// 取得所有未刪除資料
        /// 依條件查詢
        /// </summary>
        /// <param name="ParentType">ParentType</param>
        /// <param name="ParentID">ParentID</param>
        /// <returns></returns>
        public DataTable GetDataList(int NodeID, string ParentSN)
        {
            DataTable dt = new DataTable();

            using (dbEntities db = new dbEntities())
            {
                var datas = from o in db.Comm_RelFile
                            where o.NodeID == NodeID
                            && o.ParentSN == ParentSN
                            orderby o.Sort ascending
                            select o;

                dt = datas.ConvertDataTable();
            }

            return dt;
        }



        /// <summary>
        /// 取得目前最大的排序
        /// </summary>
        /// <param name="ParentType">ParentType</param>
        /// <param name="ParentID">ParentID</param>
        /// <returns></returns>
        public static int GetMaxSortByParentTypeAndParentID(int NodeID, string ParentSN, int Kind)
        {
            using (dbEntities db = new dbEntities())
            {
                var data = (from o in db.Comm_RelFile
                            where o.NodeID == NodeID
                           && o.ParentSN == ParentSN && (o.Kind ?? 0) == Kind
                            select o).Count();

                return data;
            }
        }


        /// <summary>
        /// 資料刪除
        /// </summary>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public void Delete(int original_SN)
        {
            using (dbEntities db = new dbEntities())
            {
                var data = (from o in db.Comm_RelFile
                            where o.SN == original_SN
                            select o).SingleOrDefault<Comm_RelFile>();

                //LinqTools.AutoGetProperties(data, this);

                if (data != null)
                {
                    db.Comm_RelFile.Remove(data);
                    db.SaveChanges();
                }
            }
        }

        public static void Delete(string ParentSN, int NodeID, string ContentPath)
        {
            using (dbEntities db = new dbEntities())
            {
                var datas = (from o in db.Comm_RelFile
                             where o.ParentSN == ParentSN
                             && o.NodeID == NodeID
                             select o);

                foreach (var data in datas)
                {
                    db.Comm_RelFile.Remove(data);
                }
                db.SaveChanges();


                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"" + ContentPath + "/RelFile/" + NodeID.ToString() + "/" + ParentSN.ToString());
                if (di.Exists)
                {
                    string[] files = System.IO.Directory.GetFiles(ContentPath + "/RelFile/" + NodeID.ToString() + "/" + ParentSN.ToString());
                    foreach (string s in files)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(@"" + s);
                        try
                        {
                            fi.Delete();
                        }
                        catch (System.IO.IOException e)
                        {
                            throw new Exception(e.Message);
                        }
                    }

                    // Delete a directory. Must be writable or empty.
                    try
                    {
                        System.IO.Directory.Delete(@"" + ContentPath + "/RelFile/" + NodeID.ToString() + "/" + ParentSN.ToString());
                    }
                    catch (System.IO.IOException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }



        /// <summary>
        /// 取得相關檔案 SMS ParentSN
        /// </summary>
        /// <param name="NodeID"></param>
        /// <param name="ParentSN"></param>
        /// <returns></returns>
        public static List<Comm_RelFile> GetListByParentSN(int NodeID, string ParentSN)
        {
            using (dbEntities db = new dbEntities())
            {
                var datas = (from o in db.Comm_RelFile
                             where o.NodeID == NodeID
                             && o.ParentSN == ParentSN && o.FileName != ""
                             orderby o.Sort
                             select o);
                return datas.ToList<Comm_RelFile>();
            }
        }

        #region 案件管理：新增個案
        /// <summary>
        /// 是否有相同檔名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileNameRepeat(string fileName)
        {
            bool rtn = false;
            using (dbEntities db = new dbEntities())
            {
                var Query = from x in db.Comm_RelFile
                            where x.SrcFileName.Equals(fileName)
                            select x;
                if (Query.Any())
                    rtn = true;
            }


            return rtn;
        }

        /// <summary>
        /// 設定案號並將案號存入
        /// </summary>
        /// <param name="ParentSN">暫存檔料夾檔名GUID</param>
        /// <param name="CaseNo">案號</param>
        /// <returns></returns>
        public static string SetFileCaseNoFromTemp(string ParentSN, string CaseNo)
        {
            string rtn = string.Empty;
            using (dbEntities db = new dbEntities())
            {
                var Query = from x in db.Comm_RelFile
                            where x.ParentSN.Equals(ParentSN)
                            select x;
                if (Query.Any())
                {
                    foreach (var data in Query)
                    {
                        data.CaseNo = CaseNo;
                        data.ModifyDate = DateTime.Now;
                        Comm_RelFile.Update(data);
                    }
                }
                else
                    rtn = "無檔案";
            }

            return rtn;
        }


        /// <summary>
        /// 取得所有符合案號的檔案
        /// </summary>
        /// <param name="ParentSN">暫存檔料夾檔名GUID</param>
        /// <param name="CaseNo">案號</param>
        /// <returns></returns>
        public static List<Comm_RelFile> GetAllData(string CaseNo)
        {
            List<Comm_RelFile> listRtn = new List<Comm_RelFile>();
            using (dbEntities db = new dbEntities())
            {
                var Query = from x in db.Comm_RelFile
                            where x.CaseNo.Equals(CaseNo)
                            select x;
                if (Query.Any())
                {
                    foreach (var data in Query)
                    {
                        listRtn.Add(data);
                    }
                }
            }

            return listRtn;
        }
        #endregion
    }
}
