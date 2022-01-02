using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using System.Configuration;
using Hamastar.Common;

namespace Hamastar.BusinessObject
{
    public static class DBHelper
    {
        #region 取最大號
        public static string GetMaxNo(string Haed, string YYMMDD)
        {

            using (dbEntities db = new dbEntities())
            {

                var CaseNo = (from x in db.Comm_Case where x.CaseNo.StartsWith(Haed + YYMMDD) select x.CaseNo).Max();
                if (CaseNo == null)
                    CaseNo = Haed + YYMMDD + "000001";
                else
                    CaseNo = Haed + YYMMDD + Convert.ToString(Convert.ToInt32(CaseNo.Right(6)) + 1).PadLeft(6, '0');

                return CaseNo;
            }

        }
        #endregion

        public static SqlConnection Conn
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
        }

        #region 將泛型轉成DataTable
        public static DataTable ConvertDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable tbl = new DataTable();
            PropertyInfo[] props = null;
            foreach (T item in collection)
            {
                if (props == null) //尚未初始化
                {
                    Type t = item.GetType();
                    props = t.GetProperties();
                    foreach (PropertyInfo pi in props)
                    {
                        Type colType = pi.PropertyType;
                        //針對Nullable<>特別處理
                        if (colType.IsGenericType
                            && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        //建立欄位
                        tbl.Columns.Add(pi.Name, colType);
                    }
                }
                DataRow row = tbl.NewRow();
                foreach (PropertyInfo pi in props)
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            return tbl;
        }
        #endregion


        public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> sources, string propertyName)
        {
            return orders(sources, propertyName, false);
        }

        public static IEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> sources, string propertyName)
        {
            return orders(sources, propertyName, true);
        }

        private static IEnumerable<TSource> orders<TSource>(IEnumerable<TSource> sources, string propertyName, bool isDescending)
        {
            PropertyInfo propertyInfo = typeof(TSource).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return sources;
            }
            if (isDescending)
            {
                return sources.OrderByDescending(x => propertyInfo.GetValue(x, null));
            }
            else
            {
                return sources.OrderBy(x => propertyInfo.GetValue(x, null));
            }
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> sources, string propertyName, params object[] values)
        {
            return orders(sources, propertyName, "OrderBy");
        }

        public static IQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> sources, string propertyName, params object[] values)
        {
            return orders(sources, propertyName, "OrderByDescending");
        }

        private static IQueryable<TSource> orders<TSource>(IQueryable<TSource> sources, string propertyName, string orderExpression)
        {
            var type = typeof(TSource);
            var propertyInfo = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "parameter");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(
                typeof(Queryable),
                orderExpression,
                new Type[] { type, propertyInfo.PropertyType },
                sources.Expression, Expression.Quote(orderByExp));
            return sources.Provider.CreateQuery<TSource>(resultExp);
        }


    }
}
