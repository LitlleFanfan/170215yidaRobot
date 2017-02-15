using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Reflection;

namespace ProduceComm
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 返回一个值，该值指示指定的 byte[] 对象是否出现在此byte[]中。
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool Contains(this byte[] b1, byte[] b2)
        {
            return Encoding.Default.GetString(b1).Contains(Encoding.Default.GetString(b2));
        }

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static byte[] Merge(this byte[] b1, byte[] b2)
        {
            byte[] re = new byte[b1.Length + b2.Length];
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
        public static bool Equals(this byte[] b1, byte[] b2)
        {
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
        public static byte[] Sub(this byte[] b1, int index, int length)
        {
            if (b1.Length < index + length + 1)
                return null;
            byte[] re = new byte[length];
            for (int i = 0; i < length; i++)
            {
                re[i] = b1[i + index];
            }
            return re;
        }

        /// <summary>
        /// DataTable数据转成Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ds">数据集</param>
        /// <param name="tbName">表名</param>
        /// <returns></returns>
        public static List<T> DataTableToObjList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] propinfos = null;
            if (dt == null)
                return null;
            foreach (DataRow dr in dt.Rows)
            {

                T entity = new T();
                //初始化propertyinfo
                if (propinfos == null)
                {
                    Type objtype = entity.GetType();
                    propinfos = objtype.GetProperties();
                }

                //填充entity类的属性

                foreach (PropertyInfo propinfo in propinfos)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToUpper().Equals(propinfo.Name.ToUpper()))
                        {

                            string v = null;
                            v = dr[dc.ColumnName].ToString();
                            if (!String.IsNullOrEmpty(v))
                            {
                                if (propinfo.PropertyType.Equals(typeof(DateTime?)))
                                {
                                    propinfo.SetValue(entity, (System.Nullable<DateTime>)DateTime.Parse(v), null);
                                }
                                else if (propinfo.PropertyType.Equals(typeof(System.Boolean?)))
                                {
                                    propinfo.SetValue(entity, System.Boolean.Parse(v), null);
                                }
                                else if (propinfo.PropertyType.Equals(typeof(int?)))
                                {
                                    propinfo.SetValue(entity, Convert.ToInt32(v), null);

                                }
                                else
                                {
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
    }
}
