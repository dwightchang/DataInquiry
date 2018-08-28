using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Caching;

namespace DataInquiry.Assistant.Assistant
{
    public class CodeAssistant
    {
        private static ObjectCache Cache = MemoryCache.Default;
        private static TableInfoAssistant TbAssist = new TableInfoAssistant();
        private static ColInfoAssistant ColAssist = new ColInfoAssistant();

        public static List<TableInfo> getTables(string dbConnName, string dbName, string tableName, DBConn engine)
        {
            string key = string.Format("TableInfo_{0}_{1}", dbConnName, dbName);
            object objTableList = CodeAssistant.Cache.Get(key);

            if (objTableList == null)
            {
                DataTable dt = CodeAssistant.TbAssist.getTables(dbConnName, dbName, engine);

                List<TableInfo> list = new List<TableInfo>();

                foreach (DataRow r in dt.Rows)
                {
                    TableInfo tb = new TableInfo() { TableName = r["TableName"].ToString() };
                    list.Add(tb);
                }

                if (list.Count == 0)
                {
                    // 沒有資料不cache
                    GlobalClass.debugLog("CodeAssistant", "getTables no data, key " + key);
                    return list;
                }

                CacheItemPolicy p = new CacheItemPolicy();
                p.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5);

                GlobalClass.debugLog("CodeAssistant", "getTables set data, key " + key);
                CodeAssistant.Cache.Set(key, list, p);

                return list;
            }
            else
            {
                return (List<TableInfo>)objTableList;
            }
        }

        public static List<ColumnInfo> getColumnInfo(string dbConnName, string dbName, string tableName, DBConn engine)
        {
            string key = string.Format("ColumnInfo_{0}_{1}_{2}", dbConnName, dbName, tableName);
            object objColumnList = CodeAssistant.Cache.Get(key);

            if (objColumnList == null)
            {
                DataTable dt = CodeAssistant.ColAssist.getColumnInfo(dbConnName, dbName, tableName, engine);

                List<ColumnInfo> list = new List<ColumnInfo>();

                foreach (DataRow r in dt.Rows)
                {
                    ColumnInfo tb = new ColumnInfo() { ColName = r["ColumnName"].ToString() };
                    list.Add(tb);
                }

                if (list.Count == 0)
                {
                    // 沒有資料不cache
                    GlobalClass.debugLog("CodeAssistant", "getColumnInfo no data, key " + key);
                    return list;
                }

                CacheItemPolicy p = new CacheItemPolicy();
                p.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5);

                GlobalClass.debugLog("CodeAssistant", "getColumnInfo set data, key " + key);
                CodeAssistant.Cache.Set(key, list, p);

                return list;
            }
            else
            {
                return (List<ColumnInfo>)objColumnList;
            }
        }
    }
}