using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using ProduceComm;

namespace ProduceComm.DataAccess
{
    public sealed class SqliteAccess
    {
        private SQLiteConnection con;
        private SQLiteTransaction tran = null;

        private SqliteAccess()
        {
            try
            {
                con = new SQLiteConnection(string.Format("Data Source={0};Pooling=true;FailIfMissing=false;Version=3", System.IO.Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, clsSetting.ConStr)));
            }
            catch (Exception ex)
            {
                clsSetting.loger.Error(string.Format("{0} Data Source={1};Pooling=true;FailIfMissing=false;Version=3", ex.ToString(), clsSetting.ConStr));
            }
        }

        public bool NonQuery(string sql, bool useTran = false, params SQLiteParameter[] p)
        {
            try
            {
                SQLiteCommand com = con.CreateCommand();
                if (useTran)
                {
                    if (tran == null)
                        new NullReferenceException("tran is null");
                    com.Transaction = tran;
                }
                com.CommandText = sql;
                foreach (SQLiteParameter sp in p)
                {
                    com.Parameters.Add(sp);
                }
                con.Open();
                return com.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                clsSetting.loger.Error(string.Format("{0} {1}", ex.ToString(), sql));
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable Query(string sql, params SQLiteParameter[] p)
        {
            try
            {
                SQLiteDataAdapter dap = new SQLiteDataAdapter(sql, con);
                foreach (SQLiteParameter sp in p)
                {
                    dap.SelectCommand.Parameters.Add(sp);
                }
                DataTable dt = new DataTable();
                dap.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                clsSetting.loger.Error(string.Format("{0} {1}", ex.ToString(), sql));
                return null;
            }
        }

        public void BeginTran()
        {
            try
            {
                SQLiteTransaction tran = null;
                if (con.State == ConnectionState.Open)
                {
                    tran = con.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
                clsSetting.loger.Error(ex.ToString());
            }
        }

        public bool EndTran(bool commit)
        {
            try
            {
                if (tran == null)
                    return false;

                if (commit)
                    tran.Commit();
                else
                    tran.Rollback();

                tran = null;
                return true;
            }
            catch (Exception ex)
            {
                clsSetting.loger.Error(ex.ToString());
                return false;
            }
        }

        public class CreateDataAccess
        {
            public static SqliteAccess sa = new SqliteAccess();
        }
    }
}
