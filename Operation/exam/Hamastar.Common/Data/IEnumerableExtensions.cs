using System.Collections;
using System.Data;
using System.Reflection;
using System;

namespace Hamastar.Common.Data
{
    /// <summary>
    /// IEnumerable 擴充方法
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 將 IEnumerable 轉換至 DataTable
        /// </summary>
        /// <param name="list">IEnumerable</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable(this IEnumerable list)
        {
            DataTable dt = new DataTable();
            bool schemaIsBuild = false;
            PropertyInfo[] props = null;

            foreach (object item in list)
            {
                if (!schemaIsBuild)
                {
                    props = item.GetType().GetProperties();
                    foreach (var pi in props)
                    {
                        Type colType = pi.PropertyType;
                        //針對Nullable<>特別處理
                        if (colType.IsGenericType
                            && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        //建立欄位
                        dt.Columns.Add(pi.Name, colType);


                        //  dt.Columns.Add(new DataColumn(pi.Name, pi.PropertyType));
                    }

                    schemaIsBuild = true;
                }

                var row = dt.NewRow();
                foreach (var pi in props)
                {
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                }

                dt.Rows.Add(row);
            }

            dt.AcceptChanges();
            return dt;
        }
    }
}



