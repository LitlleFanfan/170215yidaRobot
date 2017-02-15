using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using ProduceComm;
using Newtonsoft.Json;

namespace yidascan.DataAccess
{
    /// <summary>
    /// 数据访问类
    /// </summary>
    public sealed class DataAccess
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        SqlConnection con;

        /// <summary>
        /// 日志对象
        /// </summary>
        NLog.Logger loger = NLog.LogManager.GetLogger("general");
#if !DEBUG
        public string Com = clsSetting.ConStr; // @"server=AIOVQOPOOHCLF4X\SQL2008R2;database=tt2;uid=sa;pwd=sasa;Integrated Security=True";
#endif
#if DEBUG
        public string Com = @"server=WIN-BK7Q2BLK53K\SQL2005;database=Test;uid=sa;pwd=sasa;";
#endif
        public DataAccess()
        {
            con = new SqlConnection(Com);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="p">参数集</param>
        /// <returns></returns>
        public DataTable Query(string sql, params SqlParameter[] p)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter(sql, con);

                foreach (SqlParameter par in p)
                {
                    adp.SelectCommand.Parameters.Add(par);
                }

                DataTable dt = new DataTable();
                adp.Fill(dt);

                return dt;
            }
            catch (SqlException ex)
            {
                loger.Error(ex.ToString() + ", " + sql);
                return null;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="prodname">存储过程名</param>
        /// <param name="p">参数集合</param>
        /// <returns>数据表</returns>
        public DataTable ExecuteQueryProd(string prodname, params SqlParameter[] p)
        {
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = prodname;

                foreach (SqlParameter par in p)
                {
                    com.Parameters.Add(par);
                }

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(com);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                loger.Error(ex.ToString() + ", " + prodname);
                return null;
            }
        }
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="prodname">存储过程名</param>
        /// <param name="p">参数集合</param>
        /// <returns>是否有影响</returns>
        public bool ExecuteNonQueryProd(string prodname, params SqlParameter[] p)
        {
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = prodname;

                foreach (SqlParameter par in p)
                {
                    com.Parameters.Add(par);
                }

                con.Open();
                return com.ExecuteNonQuery() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                loger.Error(ex.ToString() + ", " + prodname);
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        /// <summary>
        /// 执行非查询REMARK
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="p">参数集</param>
        /// <returns></returns>
        public bool NonQuery(string sql, params SqlParameter[] p)
        {
            try
            {
                SqlCommand com = new SqlCommand(sql, con);

                foreach (SqlParameter par in p)
                {
                    com.Parameters.Add(par);
                }

                con.Open();
                return com.ExecuteNonQuery() > 0 ? true : false;
            }
            catch (SqlException ex)
            {
                loger.Error(ex.ToString() + ", " + sql);
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 执行命令集（事务）
        /// </summary>
        /// <param name="p">命令集</param>
        /// <returns>事务是否提交</returns>
        public bool NonQueryTran(List<CommandParameter> p)
        {
            SqlCommand com = new SqlCommand();
            try
            {
                com.Connection = con;
                con.Open();
                com.Transaction = con.BeginTransaction();

                foreach (CommandParameter cp in p)
                {
                    com.CommandText = cp.Sql;
                    com.Parameters.Clear();
                    foreach (SqlParameter par in cp.P)
                    {
                        com.Parameters.Add(par);
                    }
                    if (com.ExecuteNonQuery() < 0)
                    {
                        com.Transaction.Rollback();
                        return false;
                    }
                }

                com.Transaction.Commit();

                return true;
            }
            catch (SqlException ex)
            {
                com.Transaction.Rollback();
                loger.Error(ex);
                loger.Info(JsonConvert.SerializeObject(p));
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 返回 单行单列
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="values">参数集</param>
        /// <returns></returns>
        public object ExecuteScalars(string sql, params SqlParameter[] p)
        {
            try
            {
                SqlCommand com = new SqlCommand(sql, con);

                foreach (SqlParameter par in p)
                {
                    com.Parameters.Add(par);
                }

                con.Open();
                object result = com.ExecuteScalar();
                return result;
            }
            catch (SqlException ex)
            {
                loger.Error(ex.ToString() + ", " + sql);
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public class CreateDataAccess
        {
            public static DataAccess sa = new DataAccess();
        }
    }
}