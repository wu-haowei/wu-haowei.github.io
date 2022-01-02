using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using LinqKit;
using System.Data.Entity.Validation;

namespace Hamastar.BusinessObject
{
    [Serializable]
    /// <summary>
    /// 基本資料處理(增、刪、改)
    /// (ex: public partial class Employee:BaseEntity&lt;dbEntities,Employee&gt;{}
    /// </summary>
    /// <typeparam name="DB">資料庫連接(DataContext)</typeparam>
    /// <typeparam name="Entity">實際物件(Entity)</typeparam>
    public partial class BaseEntity<DB, Entity> : QueryBaseEntity<DB, Entity>
        where DB : DbContext, new()
        where Entity:class
    {
        /// <summary>
        /// 新增(批次)
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="Record">可使用逗號分開(ex:Insert(object) or Insert(object1, obejct2...) or Insert(obejct.toArray()))</param>
        /// <returns>IEnumerable新增後結果</returns>
        public static IEnumerable<Entity> Insert(params Entity[] Record)
        {
            //防呆null不執行
            if (Record == null)
            {
                return null;
            }

            //防呆長度等於0
            if (Record.Length == 0)
            {
                return Record;
            }

            List<Entity> returnvalue = new List<Entity>();
            using (DB db=new DB())
            {
                db.Database.Log = log => System.Diagnostics.Debug.Write(log);//顯示執行SQL指令

                var result = db.Set<Entity>().AddRange(Record);
                if (result != null)
                {
                    returnvalue = result.ToList();
                }
                //try
                //{
                    db.SaveChanges();
                //}
                //catch (DbEntityValidationException ex)
                //{
                //    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                //    var getFullMessage = string.Join("; ", entityError);
                //    var exceptionMessage = string.Concat(ex.Message, "errors are: ", getFullMessage);
                //  }
            }

            return returnvalue.AsEnumerable();
        }

        /// <summary>
        /// 修改(批次)
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="Record">可使用逗號分開(ex:Update(object) or Update(object1, obejct2...) or Update(obejct.toArray()))</param>
        /// <returns>IEnumerable修改後結果</returns>
        public static IEnumerable<Entity> Update(params Entity[] Record)
        {
            //防呆null不執行
            if (Record == null)
            {
                return null;
            }

            //防呆長度等於0
            if (Record.Length == 0)
            {
                return Record;
            }

            //
            //EF 在處理多筆資料異動的時候，會把資料分批處理，這會造成流量的損耗，一但面臨大資料，就很難突破效能瓶頸
            //超過十萬筆就會掛掉

            using (DB db = new DB())
            {
                db.Database.Log = log => System.Diagnostics.Debug.Write(log);//顯示執行SQL指令

                foreach (var entity in Record) {
                    db.Set<Entity>().Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            
            return Record.AsEnumerable();
        }


        /// <summary>
        /// 刪除(批次)
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="condition">條件值ex:Delete(a=>a.SN==1 &amp;&amp; a.ID=="xxx")</param>
        /// <returns>IEnumerable刪除項目</returns>
        public static IEnumerable<Entity> Delete(Expression<Func<Entity, bool>> condition)
        {
            //防呆null不執行
            if (condition == null)
            {
                return null;
            }

            List<Entity> returnvalue = new List<Entity>();
            using (DB db = new DB())
            {
                db.Database.Log = log => System.Diagnostics.Debug.Write(log);//顯示執行SQL指令

                foreach (var entity in db.Set<Entity>().AsExpandable().Where(condition))
                {
                    returnvalue.Add(entity);
                    db.Entry(entity).State = EntityState.Deleted;
                }
                db.SaveChanges();
            }

            return returnvalue.AsEnumerable();
        }
 
    }
}
