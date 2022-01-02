using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections;
using System.ComponentModel;

namespace Hamastar.Common.Data
{
    public class LinqTools
    {
        public LinqTools()
        { 
        
        }
        
        /// <summary>
        /// 找出LINQ資料物件的Primary Key欄位名稱
        /// </summary>
        /// <param name="obj">LINQ資料物件</param>
        /// <returns>PK欄位名稱字串陣列</returns>
        public static string[] FindPrimaryKeyColNames(object obj)
        {
            Type t = obj.GetType();
            List<string> pk = new List<string>();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object[] atts = pi.GetCustomAttributes(true);
                foreach (object att in atts)
                {
                    ColumnAttribute colAtt = att as ColumnAttribute;
                    if (att is ColumnAttribute && (colAtt as ColumnAttribute).IsPrimaryKey)
                        pk.Add(pi.Name);
                }
            }
            return pk.ToArray();
        }


        /// <summary>
        /// 將物件修改值併入另一個同型別物件
        /// </summary>
        /// <param name="target">欲套用新值的物件</param>
        /// <param name="change">內含新值的物件</param>
        public static void ApplyChange(object target, object change)
        {
            Type t1 = target.GetType();
            Type t2 = target.GetType();
            //檢查二者的型別必須相同
            if (t1 != t2)
                throw new ApplicationException("Type dismatch!");
            //找出Primary Key
            string[] pk = FindPrimaryKeyColNames(target);
            if (pk.Length == 0)
                throw new ApplicationException("No primary key found!");
            Dictionary<string, PropertyInfo> props =
                new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pi in t1.GetProperties())
                props.Add(pi.Name, pi);
            //比對二者的PK是否相同
            foreach (string c in pk)
            {
                PropertyInfo pi = props[c];
                object v1 = pi.GetValue(target, null);
                object v2 = pi.GetValue(change, null);
                if (!v1.Equals(v2))
                    throw new ApplicationException("Primary key dismatch!");
            }
            //比對數值，若不相同，則進行更換，但不包含PK
            foreach (PropertyInfo pi in props.Values)
            {
                if (pk.Contains(pi.Name)) continue;
                object v1 = pi.GetValue(target, null);
                object v2 = pi.GetValue(change, null);
                if (!v1.Equals(v2))
                    pi.SetValue(target, v2, null);
            }
        }

        /// <summary>
        /// 設置屬性的值 
        /// </summary>
        /// <param name="obj">設置的物件</param>
        /// <param name="name">屬性名稱</param>
        /// <param name="value">屬性值</param>
        public static void SetProperty(object obj, string name, object value)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(name);
            object objValue = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(obj, objValue, null);
        }

        /// <summary>
        /// 讀取屬性的值 
        /// </summary>
        /// <param name="obj">讀取的物件</param>
        /// <param name="name">屬性名稱</param>
        /// <returns></returns>
        public static object GetProperty(object obj, string name)
        {
            //bindingFlags 
            PropertyInfo propertyInfo = obj.GetType().GetProperty(name);
            if (propertyInfo != null)
                return propertyInfo.GetValue(obj, null);
            else
                return null;
        }

        //REF: http://3.ly/y8rA
        //註: .NET 3.5有個IEnumerable.CopyToDataTable<T>()，但T只接受DataRow(感覺有點莊孝維)
        public static DataTable LinqQueryToDataTable<T>(IEnumerable<T> query)
        {
            DataTable tbl = new DataTable();
            PropertyInfo[] props = null;

            foreach (T item in query)
            {
                //尚未初始化
                if (props == null) 
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

        /// <summary>
        /// 自動帶入屬性值至另一個物件的屬性
        /// </summary>
        /// <param name="target">欲套用新值的物件</param>
        /// <param name="source">來源</param>
        public static void AutoGetProperties(object target, object source)
        {
            Type type = target.GetType();
            foreach (PropertyInfo p in type.GetProperties())
            {
                p.SetValue(target, LinqTools.GetProperty(source, p.Name), null);
            }
        }



        ///// <summary> 
        ///// Copies the data of one object to another. The target object gets properties of the first.  
        ///// Any matching properties (by name) are written to the target. 
        ///// </summary> 
        ///// <param name="source">The source object to copy from</param> 
        ///// <param name="target">The target object to copy to</param> 
        ///// DeepCopy<T>(T copyTo, T copyFrom)
        //public static void DeepCopy(object target, object source)
        //{
        //    CopyObjectData(source, target, String.Empty, BindingFlags.Public | BindingFlags.Instance);
        //}


        ///// <summary> 
        ///// Copies the data of one object to another. The target object gets properties of the first.  
        ///// Any matching properties (by name) are written to the target. 
        ///// </summary> 
        ///// <param name="source">The source object to copy from</param> 
        ///// <param name="target">The target object to copy to</param> 
        ///// <param name="excludedProperties">A comma delimited list of properties that should not be copied</param> 
        ///// <param name="memberAccess">Reflection binding access</param> 
        //public static void CopyObjectData(object source, object target, string excludedProperties, BindingFlags memberAccess)
        //{
        //    string[] excluded = null;
        //    if (!string.IsNullOrEmpty(excludedProperties))
        //    {
        //        excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    }

        //    MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
        //    foreach (MemberInfo Field in miT)
        //    {
        //        string name = Field.Name;

        //        // Skip over excluded properties 
        //        if (string.IsNullOrEmpty(excludedProperties) == false
        //            && excluded.Contains(name))
        //        {
        //            continue;
        //        }


        //        if (Field.MemberType == MemberTypes.Field)
        //        {
        //            FieldInfo sourcefield = source.GetType().GetField(name);
        //            if (sourcefield == null) { continue; }

        //            object SourceValue = sourcefield.GetValue(source);
        //            ((FieldInfo)Field).SetValue(target, SourceValue);
        //        }
        //        else if (Field.MemberType == MemberTypes.Property)
        //        {
        //            PropertyInfo piTarget = Field as PropertyInfo;
        //            PropertyInfo sourceField = source.GetType().GetProperty(name, memberAccess);
        //            if (sourceField == null) { continue; }

        //            if (piTarget.CanWrite && sourceField.CanRead)
        //            {
        //                object targetValue = piTarget.GetValue(target, null);
        //                object sourceValue = sourceField.GetValue(source, null);

        //                if (sourceValue == null) { continue; }

        //                if (sourceField.PropertyType.IsArray
        //                    && piTarget.PropertyType.IsArray
        //                    && sourceValue != null)
        //                {
        //                    CopyArray(source, target, memberAccess, piTarget, sourceField, sourceValue);
        //                }
        //                else
        //                {
        //                    CopySingleData(source, target, memberAccess, piTarget, sourceField, targetValue, sourceValue);
        //                }
        //            }
        //        }
        //    }
        //}

        //private static void CopySingleData(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object targetValue, object sourceValue)
        //{
        //    //instantiate target if needed 
        //    if (targetValue == null
        //        && piTarget.PropertyType.IsValueType == false
        //        && piTarget.PropertyType != typeof(string))
        //    {
        //        if (piTarget.PropertyType.IsArray)
        //        {
        //            targetValue = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
        //        }
        //        else
        //        {
        //            targetValue = Activator.CreateInstance(piTarget.PropertyType);
        //        }
        //    }

        //    if (piTarget.PropertyType.IsValueType == false
        //        && piTarget.PropertyType != typeof(string))
        //    {
        //        CopyObjectData(sourceValue, targetValue, "", memberAccess);
        //        piTarget.SetValue(target, targetValue, null);
        //    }
        //    else
        //    {
        //        if (piTarget.PropertyType.FullName == sourceField.PropertyType.FullName)
        //        {
        //            object tempSourceValue = sourceField.GetValue(source, null);
        //            piTarget.SetValue(target, tempSourceValue, null);
        //        }
        //        else
        //        {
        //            CopyObjectData(piTarget, target, "", memberAccess);
        //        }
        //    }
        //}

        //private static void CopyArray(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object sourceValue)
        //{
        //    int sourceLength = (int)sourceValue.GetType().InvokeMember("Length", BindingFlags.GetProperty, null, sourceValue, null);
        //    Array targetArray = Array.CreateInstance(piTarget.PropertyType.GetElementType(), sourceLength);
        //    Array array = (Array)sourceField.GetValue(source, null);

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        object o = array.GetValue(i);
        //        object tempTarget = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
        //        CopyObjectData(o, tempTarget, "", memberAccess);
        //        targetArray.SetValue(tempTarget, i);
        //    }
        //    piTarget.SetValue(target, targetArray, null);
        //} 


        //public static void DeepCopy<T>(T copyTo, T copyFrom)
        //{
        //    //get properties info from source:
        //    PropertyInfo[] propertiesFrom = copyFrom.GetType().GetProperties();

        //    //loop through the properties info:
        //    foreach (PropertyInfo propertyFrom in propertiesFrom)
        //    {
        //        //get value from source:
        //        var valueFrom = propertyFrom.GetValue(copyFrom, null);

        //        //get property from destination
        //        var propertyTo = copyTo.GetType().GetProperty(propertyFrom.Name);


        //        if (propertyTo != null && valueFrom != null)
        //        //a bit of extra validation
        //        {
        //            //the property is an entity collection:
        //            if (valueFrom.GetType().Name.Contains("EntityCollection"))
        //            {
        //                //get value from destination
        //                var valueTo = copyTo.GetType().GetProperty(
        //                               propertyFrom.Name).GetValue(copyTo, null);

        //                //get collection generic type:
        //                Type genericType =
        //                  propertyTo.PropertyType.GetGenericArguments()[0];


        //                //get list source from source:
        //                IListSource colFrom = (IListSource)valueFrom;

        //                //get list source from destination:
        //                IListSource colTo = (IListSource)valueTo;

        //                //loop through list source:
        //                foreach (dynamic b in colFrom.GetList())
        //                {
        //                    //create instance of the generic type:
        //                    dynamic c = (dynamic)Activator.CreateInstance(genericType);

        //                    //copy source into this instance:
        //                    DeepCopy<dynamic>(b, c);

        //                    //add the instance into destination entity collection:
        //                    colTo.GetList().Add(c);
        //                }
        //            }

        //            // do not copy if the property:
        //            //is  entity object,
        //            //is entity reference,
        //            //entity state,
        //            //entity key
        //            else if (propertyTo.PropertyType.BaseType.Name.Contains("EntityObject")
        //                || valueFrom.GetType().Name.Contains("EntityReference")
        //                || valueFrom.GetType().Name.Contains("EntityState")
        //                || valueFrom.GetType().Name.Contains("EntityKey"))
        //            {
        //                //do nothing;
        //            }
        //            else // set the value of the destination property:
        //                propertyTo.SetValue(copyTo, valueFrom, null);
        //        }
        //    }
        //} 
    }
}
