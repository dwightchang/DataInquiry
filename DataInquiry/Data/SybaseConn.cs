using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace DataInquiry.Assistant.Data
{
    class SybaseConn : DBConn
    {
        private string _dbstr;
        private OleDbConnection _conn = null;
        //private OleDbTransaction _tran = null;

        public SybaseConn(string dbstr) : base(dbstr)
        {
        }

        public override void init(string dbstr)
        {
            try
            {
                _dbstr = dbstr;

                _conn = new OleDbConnection(dbstr);
                _conn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }

        public override int executeSQL(string sql)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void startTransaction(System.Data.IsolationLevel lvl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void commit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void rollback()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet getDataSet(string sql, string name)
        {
            OleDbDataAdapter adp = new OleDbDataAdapter();
            OleDbCommand cmd = createCommand(sql);

            System.Data.DataSet ds = new System.Data.DataSet();
            adp.Fill(ds, name);

            return ds;
        }

        public OleDbCommand createCommand(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, this._conn);            
            cmd.CommandTimeout = 1200;            

            return cmd;
        }

        public override bool updateDataSet(System.Data.DataSet dataSet)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool isOpen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void open()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void cancelCommand()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string getMessage()
        {
            return "";
        }
    }
}
