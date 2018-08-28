using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DataInquiry.Assistant
{
    public abstract class DBConn
    {
        public DBConn(string dbstr)
        {
            init(dbstr);
        }

        abstract public void init(string dbstr);
        abstract public int executeSQL(string sql);
        public DataTable getData(string sql)
        {
            DataSet ds = getDataSet(sql, "temp");
            return ds.Tables[0];
        }

        public string Dbstr { get; set; }

        abstract public DataSet getDataSet(string sql, string name);
        abstract public bool updateDataSet(DataSet dataSet);
        abstract public void startTransaction(System.Data.IsolationLevel lvl);
        abstract public void commit();
        abstract public void rollback();
        abstract public void open();
        abstract public void close();
        abstract public bool isOpen();
        abstract public void cancelCommand();
        abstract public string getMessage();

        virtual public DataSet getColumns(string tablename, string dbname) { return null; }
    }
}
