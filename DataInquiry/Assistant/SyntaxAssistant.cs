using System.Collections;
using System.Text.RegularExpressions;

namespace DataInquiry.Assistant
{
    public class SyntaxAssistant
    {
        public ArrayList getAssist(string sql)
        {
            ArrayList r = new ArrayList();
            sql = sql.ToLower();

            string selectSql = sql;
            int lastSelIdx = sql.LastIndexOf("select");
            if (lastSelIdx > 0)
            {
                selectSql = sql.Substring(lastSelIdx);
            }

            Match m, m2;

            // with(nolock)
            // [ex.] from Users w
            m = Regex.Match(selectSql, "\\sfrom\\s+[_0-9a-z]+\\s+[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(selectSql, "\\sjoin\\s+[_0-9a-z]+\\s+[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);

            if (m.Success || m2.Success)
            {
                r.Add("with(nolock)");                
            }

            // with(nolock)
            // [ex.] from Users u w
            m = Regex.Match(selectSql, "\\sfrom\\s+[_0-9a-z]+\\s+[a-z]+\\s+[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(selectSql, "\\sjoin\\s+[_0-9a-z]+\\s+[a-z]+\\s+[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);

            if (m.Success || m2.Success)
            {
                r.Add("with(nolock)");
            }

            // with(nolock)
            // [ex.] from Users as u with(nolock)
            m = Regex.Match(selectSql, "\\sfrom\\s+[_0-9a-z]+\\s+as\\s+[a-z]+\\s[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(selectSql, "\\sjoin\\s+[_0-9a-z]+\\s+as\\s+[a-z]+\\s[w][i]?[t]?[h]?\\z", RegexOptions.Singleline);

            if (m.Success || m2.Success)
            {
                r.Add("with(nolock)");
            }

            // where - select * from users where
            string sql2 = selectSql.Replace("with(nolock)", "");
            m = Regex.Match(sql2, "\\sfrom\\s+[_0-9a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(sql2, "\\sjoin\\s+[_0-9a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            if (m.Success || m2.Success)
            {                
                r.Add("where");
            }

            // where - select * from users u where
            m = Regex.Match(sql2, "\\sfrom\\s+[_0-9a-z]+\\s+[a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(sql2, "\\sjoin\\s+[_0-9a-z]+\\s+[a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            if (m.Success || m2.Success)
            {
                r.Add("where");
            }

            // where - select * from users as u where
            m = Regex.Match(sql2, "\\sfrom\\s+[_0-9a-z]+\\s+as\\s+[a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            m2 = Regex.Match(sql2, "\\sjoin\\s+[_0-9a-z]+\\s+as\\s+[a-z]+\\s+[w][h]?[e]?[r]?\\z", RegexOptions.Singleline);
            if (m.Success || m2.Success)
            {
                r.Add("where");
            }

            // order by - from xxxxx or            
            m = Regex.Match(sql2, "\\sfrom\\s+.+[o][r]?\\z", RegexOptions.Singleline);
            if (m.Success)
            {
                r.Add("order by");
            }            

            return r;
        }
    }
}