using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DataInquiry.Assistant.Data
{
    class SqlConn : DBConn
    {
        //private string _dbstr;
        public SqlConnection _conn = null;
        private SqlTransaction _tran = null;
        public SqlCommand _cmd = null;
        public string Message { get; set; }

        public SqlConn(string dbstr) : base(dbstr)
        {
        }

        public override void init(string dbstr)
        {
            try
            {
                Dbstr = dbstr;
                
                _conn = new SqlConnection(dbstr);
                _conn.InfoMessage += _conn_InfoMessage;
                this.Message = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }

        void _conn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            if (e.Message.StartsWith("Changed database context to "))
            {
                return;
            }

            this.Message += e.Message + "\r\n";
        }

        private void checkConnState()
        {
            if (_conn.State != ConnectionState.Closed)
            {
                throw new Exception("connection is not closed: " + _conn.State.ToString());
            }
        }

        override public int executeSQL(string sql)
        {
            try
            {
                this.Message = "";
                _cmd = createCommand(sql);
                int rows = _cmd.ExecuteNonQuery();
                
                return rows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_tran == null)
                {
                    _conn.Close();
                    _cmd = null;
                }
            }            
        }

        public SqlCommand createCommand(string sql)
        {
            if (_conn.State == ConnectionState.Closed)
            {
                _conn.Open();
            }            

            SqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 1200;
            cmd.Transaction = _tran;

            return cmd;
        }

        public new DataTable getData(string sql)
        {            
            DataSet ds = getDataSet(sql, "temp");            

            return ds.Tables[0];
        }

        override public DataSet getDataSet(string sql, string name)
        {
            //GlobalClass.errorLog("sql: " + sql);

            try
            {
                this.Message = "";
                _cmd = createCommand(sql);
                SqlDataAdapter adp = new SqlDataAdapter(_cmd);

                DataSet ds = new DataSet();
                adp.Fill(ds, name);

                if(ds.Tables.Count > 0)
                {
                    ds.Tables[0].ExtendedProperties.Add("SQL", sql);
                }                

                return ds;
            }
            catch(SqlException sex)
            {
                StringBuilder sb = new StringBuilder();
                foreach(SqlError err in sex.Errors)
                {
                    // line -1 因為第一行是 use [dbxxx]
                    sb.AppendFormat("Line {0}, {1}\r\n", (err.LineNumber - 1).ToString(), err.Message);
                }

                throw new Exception (sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + "\n" + sql);
            }
            finally
            {
                _cmd = null;
                _conn.Close();                
            }
        }


        /// <summary>
        /// 將DataSet更新至資料庫
        /// </summary>
        /// <param name="ds">要更新的DataSet</param>
        /// <returns>true: 成功; false: 失敗</returns>
        override public bool updateDataSet(DataSet dataSet)
        {            
            try
            {
                this.Message = "";
                string sql = dataSet.Tables[0].ExtendedProperties["SQL"].ToString();

                SqlDataAdapter adapter = getAdapter(sql);
                string tableName = dataSet.Tables[0].TableName;

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                if (tableName != null)
                    adapter.Update(dataSet, tableName);
                else
                    adapter.Update(dataSet);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _conn.Close();
            }
        }

        public SqlDataAdapter getAdapter(string sql)
        {
            SqlCommand cmd = createCommand(sql);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            return adapter;
        }

        override public void startTransaction(System.Data.IsolationLevel lvl)
        {
            if (_conn.State == ConnectionState.Closed)
            {
                _conn.Open();
            }
            _tran = _conn.BeginTransaction(lvl);
        }

        override public void commit()
        {
            _tran.Commit();
            _tran = null;
        }

        override public void rollback()
        {
            _tran.Rollback();
            _tran = null;
        }

        public override void open()
        {
            _conn.Open();
        }

        override public void close()
        {
            _conn.Close();
        }

        override public bool isOpen()
        {
            if (_conn.State == ConnectionState.Open)
            {
                return true;
            }

            return false;
        }

        public override void cancelCommand()
        {
            if (this._cmd != null)
            {
                this._cmd.Cancel();
            }
        }

        public override DataSet getColumns(string tablename, string dbname)
        {
            string sql = string.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS " +
            "where TABLE_NAME = '{0}' and TABLE_CATALOG='{1}' " +
            "order by ORDINAL_POSITION ", tablename, dbname);

            sql = applyUsedDatabase(sql, dbname);

            DataSet ds = this.getDataSet(sql, "temp");
            return ds;
        }

        private string applyUsedDatabase(string sql, object pDbName)
        {
            if (pDbName != null && pDbName.ToString() != "")
            {
                sql = "USE [" + pDbName.ToString().Trim() + "];  \r\n" + sql;
            }

            return sql;
        }

        public override string getMessage()
        {
            string message = this.Message;
            this.Message = "";

            return message;
        }
    }
}
