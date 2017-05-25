using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace yidascan.DataAccess {
    public sealed class DataAccess {
        /// <summary>
        /// 日志对象
        /// </summary>
        NLog.Logger loger = NLog.LogManager.GetLogger("general");

#if !DEBUG
        private string CONNECTION_S = ProduceComm.clsSetting.ConStr; // @"server=AIOVQOPOOHCLF4X\SQL2008R2;database=tt2;uid=sa;pwd=sasa;Integrated Security=True";
#endif
#if DEBUG
        // private const string CONNECTION_S = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=yada_stacking;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";
        private const string CONNECTION_S = @"server=.;database=yada_stacking;uid=sa;pwd=sasa;Integrated Security=True";
#endif

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="p">参数集</param>
        /// <returns></returns>
        public DataTable Query(string sql, params SqlParameter[] p) {
            using (var con = new SqlConnection(CONNECTION_S)) {
                try {
                    con.Open();
                    using (var adp = new SqlDataAdapter(sql, con)) {
                        foreach (SqlParameter par in p) {
                            adp.SelectCommand.Parameters.Add(par);
                        }

                        var dt = new DataTable();
                        adp.Fill(dt);

                        return dt;
                    }
                } catch (SqlException ex) {
                    loger.Error(ex.ToString() + ", " + sql);
                    return null;
                }
            }
        }

        /// <summary>
        /// 执行非查询REMARK
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="p">参数集</param>
        /// <returns></returns>
        public bool NonQuery(string sql, params SqlParameter[] p) {
            using (var con = new SqlConnection(CONNECTION_S)) {
                try {
                    con.Open();
                    using (var com = new SqlCommand(sql, con)) {
                        foreach (SqlParameter par in p) {
                            com.Parameters.Add(par);
                        }

                        return com.ExecuteNonQuery() > 0;
                    }
                } catch (SqlException ex) {
                    loger.Error(ex.ToString() + ", " + sql);
                    return false;
                }
            }
        }

        /// <summary>
        /// 执行命令集（事务）
        /// </summary>
        /// <param name="p">命令集</param>
        /// <returns>事务是否提交</returns>
        public bool NonQueryTran(List<CommandParameter> p) {
            using (var con = new SqlConnection(CONNECTION_S))
            using (var com = new SqlCommand()) {
                try {
                    con.Open();
                    com.Connection = con;
                    com.Transaction = con.BeginTransaction();

                    foreach (CommandParameter cp in p) {
                        com.CommandText = cp.Sql;
                        com.Parameters.Clear();
                        foreach (SqlParameter par in cp.P) {
                            com.Parameters.Add(par);
                        }
                        if (com.ExecuteNonQuery() < 0) {
                            com.Transaction.Rollback();
                            return false;
                        }
                    }
                    com.Transaction.Commit();
                    return true;
                } catch (SqlException ex) {
                    com.Transaction.Rollback();
                    loger.Error($"!来源: {nameof(NonQueryTran)}, 数据库操作失败: {ex}");
                    loger.Info(JsonConvert.SerializeObject(p));
                    return false;
                }
            }
        }

        /// <summary>
        /// 返回 单行单列
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="p">todo: describe p parameter on ExecuteScalars</param>
        /// <returns></returns>
        public object ExecuteScalars(string sql, params SqlParameter[] p) {
            using (var con = new SqlConnection(CONNECTION_S))
            using (var com = new SqlCommand(sql, con)) {
                try {
                    foreach (SqlParameter par in p) {
                        com.Parameters.Add(par);
                    }

                    con.Open();
                    return com.ExecuteScalar();
                } catch (SqlException ex) {
                    loger.Error(ex.ToString() + ", " + sql);
                    return null;
                }
            }
        }
        public class CreateDataAccess {
            public static DataAccess sa = new DataAccess();
        }
    }

}