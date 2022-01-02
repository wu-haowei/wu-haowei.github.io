using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Reflection;
using System.IO;

namespace Hamastar.BusinessObject
{
    [Serializable]
    /// <summary>
    /// 基本資料處理(查詢)
    /// (ex: public partial class Employee:BaseEntity&lt;dbEntities,Employee&gt;{}
    /// </summary>
    /// <typeparam name="DB">資料庫連接(DataContext)</typeparam>
    /// <typeparam name="Entity">實際物件(Entity)</typeparam>
    public partial class QueryBaseEntity<DB, Entity>
        where DB : DbContext, new()
        where Entity : class
    {

        /// <summary>
        /// 依條件取值
        /// </summary>
        /// <param name="condition">條件內容(ex:GetList(a=>a.ID=="xxx" &amp;&amp; a.Name!="ooo"))</param>
        /// <returns>IQueryable項目</returns>
        public static IQueryable<Entity> GetList(Expression<Func<Entity, bool>> condition)
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

                var query = db.Set<Entity>().AsExpandable().Where(condition).AsNoTracking();
                if (query != null || query.Any())
                {

                    returnvalue = query.ToList();
                }
            }

            return returnvalue.AsQueryable();
        }


        public static IQueryable<TResult> GetList<TResult>(Expression<Func<Entity, bool>> condition, Expression<Func<Entity, TResult>> selector)
        {
            //防呆null不執行
            if (condition == null)
            {
                return null;
            }

            List<TResult> returnvalue = new List<TResult>();
            using (DB db = new DB())
            {
                db.Database.Log = log => System.Diagnostics.Debug.Write(log);//顯示執行SQL指令

                var query = db.Set<Entity>().AsNoTracking().AsExpandable().Where(condition).Select(selector);
                if (query != null || query.Any())
                {
                    returnvalue = query.ToList();
                }
            }

            return returnvalue.AsQueryable();
        }


        /// <summary>
        /// 依條件進行查詢(只取回特定欄位)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="condition">條件內容(ex:GetList(a=>a.ID=="xxx" &amp;&amp; a.Name!="ooo"))</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> GetList<TResult>(Expression<Func<Entity, bool>> condition, Expression<Func<Entity, int, TResult>> selector)
        {
            //防呆null不執行
            if (condition == null)
            {
                return null;
            }

            List<TResult> returnvalue = new List<TResult>();
            using (DB db = new DB())
            {
                db.Database.Log = log => System.Diagnostics.Debug.Write(log);//顯示執行SQL指令

                var query = db.Set<Entity>().AsNoTracking().AsExpandable().Where(condition).Select(selector);
                if (query != null || query.Any())
                {
                    returnvalue = query.ToList();
                }
            }

            return returnvalue.AsQueryable();
        }

        //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, TResult>> selector);
        //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);

        //public static IQueryable<tresult> Select<tsource, tresult>(this IQueryable<tsource> source, Expression<func<tsource, tresult="">> selector)
        //{
        //    if (source == null)
        //        throw Error.ArgumentNull("source");
        //    if (selector == null)
        //        throw Error.ArgumentNull("selector");
        //    return source.Provider.CreateQuery<tresult>(
        //        Expression.Call(
        //            null,
        //            ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TResult)),
        //            new Expression[] { source.Expression, Expression.Quote(selector) }
        //            ));
        //}

        //public static IQueryable<tresult> Select<tsource, tresult>(this IQueryable<tsource> source, Expression<func<tsource, int,="" tresult="">> selector)
        //{
        //    if (source == null)
        //        throw Error.ArgumentNull("source");
        //    if (selector == null)
        //        throw Error.ArgumentNull("selector");
        //    return source.Provider.CreateQuery<tresult>(
        //        Expression.Call(
        //            null,
        //            ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TResult)),
        //            new Expression[] { source.Expression, Expression.Quote(selector) }
        //            ));
        //}

        ////public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);


        /// <summary>
        /// 依條件只取第一筆值
        /// </summary>
        /// <param name="condition">條件內容(ex:GetList(a=>a.ID=="xxx" &amp;&amp; a.Name!="ooo"))</param>
        /// <returns>Entity或null</returns>
        public static Entity GetSingle(Expression<Func<Entity, bool>> condition)
        {
            //防呆null不執行
            if (condition == null)
            {
                return null;
            }

            return GetList(condition).FirstOrDefault();
        }
    }
}
