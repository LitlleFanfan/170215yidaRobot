using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Reflection;

namespace ProduceComm {
    /// <summary>
    /// 公共方法
    /// </summary>
    public static class Helper {
        /// <summary>
        /// 返回一个值，该值指示指定的 byte[] 对象是否出现在此byte[]中。
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool Contains(this byte[] b1, byte[] b2) {
            return Encoding.Default.GetString(b1).Contains(Encoding.Default.GetString(b2));
        }

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static byte[] Merge(this byte[] b1, byte[] b2) {
            var re = new byte[b1.Length + b2.Length];
            b1.CopyTo(re, 0);
            b2.CopyTo(re, b1.Length);
            return re;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool Equals(this byte[] b1, byte[] b2) {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i]) return false;
            return true;
        }

        /// <summary>
        /// 截取
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Sub(this byte[] b1, int index, int length) {
            if (b1.Length < index + length + 1)
                return null;
            var re = new byte[length];
            for (int i = 0; i < length; i++) {
                re[i] = b1[i + index];
            }
            return re;
        }

        [Obsolete("use DataTableExtensions instead.")]
        public static List<T> DataTableToObjList<T>(DataTable dt) where T : new() {
            List<T> list = new List<T>();
            PropertyInfo[] propinfos = null;
            if (dt == null)
                return null;
            foreach (DataRow dr in dt.Rows) {

                T entity = new T();
                //初始化propertyinfo
                if (propinfos == null) {
                    Type objtype = entity.GetType();
                    propinfos = objtype.GetProperties();
                }

                //填充entity类的属性

                foreach (PropertyInfo propinfo in propinfos) {
                    foreach (DataColumn dc in dt.Columns) {
                        if (dc.ColumnName.ToUpper().Equals(propinfo.Name.ToUpper())) {

                            string v = null;
                            v = dr[dc.ColumnName].ToString();
                            if (!String.IsNullOrEmpty(v)) {
                                if (propinfo.PropertyType.Equals(typeof(DateTime?))) {
                                    propinfo.SetValue(entity, (System.Nullable<DateTime>)DateTime.Parse(v), null);
                                } else if (propinfo.PropertyType.Equals(typeof(System.Boolean?))) {
                                    propinfo.SetValue(entity, System.Boolean.Parse(v), null);
                                } else if (propinfo.PropertyType.Equals(typeof(int?))) {
                                    propinfo.SetValue(entity, Convert.ToInt32(v), null);

                                } else {
                                    propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);

                                }
                                break;
                            }
                        }
                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 重复运行某个函数。
        /// </summary>
        /// <param name="func">需要重复运行的函数</param>
        /// <param name="times">重复次数</param>
        /// <param name="delay">每次运行之间的间隔</param>
        public static bool retry(Func<bool> func, int times = 1, int delay = 0) {
            var counter = times;
            while (func != null && counter-- > 0) {
                if (func()) { return true; }
                Thread.Sleep(delay);
            }
            return false;
        }

        /// <summary>
        /// 重复运行某个函数。
        /// </summary>
        /// <param name="func">需要重复运行的函数</param>
        /// <param name="maxtimes">持续时间，毫秒</param>
        /// <param name="delay">每次运行之间的间隔</param>
        public static string retryTime(Func<bool> func, int maxtimes = 300, int delay = 30) {
            string str = string.Empty;
            DateTime dt = DateTime.Now;
            while (func != null && dt.Subtract(DateTime.Now).Duration().TotalMilliseconds < maxtimes) {
                str = $"{str} {DateTime.Now.ToString("mm:ss fff")} {func()}";
                Thread.Sleep(delay);
            }
            return str;
        }
    }
}

/// <summary>
/// var items = dt.ToList<Item>();
/// 
/// //or
/// var mappings = new Dictionary<string, string>();
/// 
/// //keys are the properties.
/// 
/// mappings.Add("ItemId", "item_id");
/// mappings.Add("ItemName ", "item_name");
/// mappings.Add("Price ", "price);
/// 
/// var items = dt.ToList<Item>(mappings);
/// </summary>
public static class DataTableExtensions {
    public static IList<T> ToList<T>(this DataTable table) where T : new() {
        IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
        IList<T> result = new List<T>();

        foreach (var row in table.Rows) {
            var item = CreateItemFromRow<T>((DataRow)row, properties);
            result.Add(item);
        }

        return result;
    }

    public static IList<T> ToList<T>(this DataTable table, Dictionary<string, string> mappings) where T : new() {
        IList<PropertyInfo> properties = typeof(T)
            .GetProperties()
            .ToList();
        IList<T> result = new List<T>();

        foreach (var row in table.Rows) {
            var item = CreateItemFromRow<T>((DataRow)row, properties, mappings);
            result.Add(item);
        }

        return result;
    }

    private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new() {
        var item = new T();
        foreach (var property in properties) {
            property.SetValue(item, row[property.Name], null);
        }
        return item;
    }

    private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties, Dictionary<string, string> mappings) where T : new() {
        var item = new T();
        foreach (var property in properties) {
            if (mappings.ContainsKey(property.Name))
                property.SetValue(item, row[mappings[property.Name]], null);
        }
        return item;
    }
}
