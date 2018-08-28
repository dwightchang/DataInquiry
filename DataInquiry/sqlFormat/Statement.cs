using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DataInquiry.Assistant.sqlFormat
{
    public class Statement
    {
        private string _text;
        private ArrayList _stmts;
        public int _depth = 0;

        public Statement()
        {
            _stmts = new ArrayList();
        }

        public void readSql(string pSql)
        {            
            string text = pSql.Replace("\n", " ");

            int[] index;
            index = findPattern(text);

            if (index[0] == -1)
            {
                _text = text;
            }
            else
            {
                Statement stmt;

                stmt = new Statement();
                stmt._depth = this._depth;
                stmt.readSql(text.Substring(0, index[0]));                
                _stmts.Add(stmt);

                stmt = new Statement();
                stmt._depth = this._depth + 1;
                stmt.readSql(text.Substring(index[0], index[1] - index[0] + 1));                
                _stmts.Add(stmt);

                stmt = new Statement();
                stmt._depth = this._depth;
                stmt.readSql(text.Substring(index[1] + 1));                
                _stmts.Add(stmt);
            }

            //_text = pSql.Trim().Replace("\n", " ");
        }

        private int [] findPattern(string text)
        {
            text = text.ToLower();

            int[] index = new int[2];
            index[0] = -1;
            index[1] = -1;

            Match match = null;

            // pattern 1: from 後面的子查詢
            match = Regex.Match(text, "from[\\s][(]", RegexOptions.Singleline);
            if (match.Success)
            {          
                int startIdx = match.Index + match.Length - 1;
                int endIdx = findEndParentheses(text, startIdx);

                if (endIdx != -1)
                {
                    index[0] = startIdx;
                    index[1] = endIdx;
                }
                
            }

            // pattern 2: join 後面的子查詢
            match = Regex.Match(text, "join[\\s][(]", RegexOptions.Singleline);
            if (match.Success)
            {
                int startIdx = match.Index + match.Length - 1;
                int endIdx = findEndParentheses(text, startIdx);

                if (endIdx != -1 && startIdx < index[0])  // 若有，取更外圍的pattern
                {
                    index[0] = startIdx;
                    index[1] = endIdx;
                }
                
            }

            return index;
        }

        private int findEndParentheses(string text, int startIdx)
        {
            int count = 1;
            for (int i = startIdx + 1; i < text.Length; i++)
            {
                if (text[i] == '(')
                {
                    ++count;
                }
                if (text[i] == ')')
                {
                    --count;
                }

                if (count == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private string preProcess(string text, string from, string to)
        {
            MatchCollection ms = Regex.Matches(text, from, RegexOptions.IgnoreCase);
            string lastMatchString = "";
            for (int i = 0; i < ms.Count; i++)
            {
                if (lastMatchString.Equals(ms[i].Value) == false)
                {
                    lastMatchString = ms[i].Value;

                    text = text.Replace(ms[i].Value, to);
                }
            }

            return text;
        }

        public string formatSql()
        {
            if (_text != null)
            {
                //_text = _text.Replace("left join", "left_join");

                //MatchCollection ms = Regex.Matches(_text, "left join", RegexOptions.IgnoreCase);
                //string lastMatchString = "";
                //for (int i = 0; i < ms.Count; i++)
                //{
                //    if (lastMatchString.Equals(ms[i].Value) == false)
                //    {
                //        lastMatchString = ms[i].Value;

                //        _text = _text.Replace(ms[i].Value, "left§join");
                //    }
                //}

                _text = preProcess(_text, "left join", "left§join");
                _text = preProcess(_text, "union all", "union§all");

                insertNewLine("from");
                insertNewLine("order by");
                insertNewLine("left§join");
                insertNewLine("union§all");
                insertNewLine("join");
                insertNewLine("where");
                insertNewLine("select");

                int maxLen = 80;
                Match match = Regex.Match(_text, "select[\\s].{0,}[\\s]from", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string selectString = _text.Substring(match.Index, match.Length);
                    string[] fields = selectString.Split(",".ToCharArray());

                    int accLen = 0;
                    for (int i = 0; i < fields.Length; i++)
                    {
                        accLen += fields[i].Length;

                        if (accLen > maxLen)
                        {
                            insertNewLineAfterComma(i);
                            accLen = 0;
                        }

                        if (fields[i].Length > maxLen)
                        {
                            insertNewLineAfterComma(i+1);
                            accLen = 0;
                        }                        
                    }

                }

                _text = _text.Replace("left§join", "Left Join");
                _text = _text.Replace("union§all", "Union all");

                return _text;
            }
            else
            {
                string statement = "";
                for (int i = 0; i < _stmts.Count; i++)
                {
                    Statement stmt = (Statement) _stmts[i];
                    statement += stmt.formatSql();
                }

                return statement;
            }
            
        }

        private void insertNewLineAfterComma(int count)
        {
            if (count == 0)
            {
                return;
            }

            int commaCnt = 0;
            int cnt = 0;
            int idx = 0;

            do
            {
                idx = _text.IndexOf(",", idx+1);
                if (idx >= 0)
                {
                    commaCnt++;
                }
                else
                {
                    return;
                }

                if (commaCnt == count)
                {
                    _text = _text.Insert(idx + 1, "\n".PadRight((this._depth+1)*2+1, ' '));
                    return;
                }

                ++cnt;
            } while (cnt < 500);
        }

        private void insertNewLine(string keyword)
        {
            //int idx = _text.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase);
            int idx = 0;

            keyword = "[\\s\\t]+" + keyword + "[\\s\\t]+";
            Match m = Regex.Match(_text, keyword, RegexOptions.IgnoreCase);

            int cnt = 0;
            while (m.Success)
            {
                int intSpace = _depth * 2 + 1;
                _text = _text.Insert(idx + m.Index + 1, "\n".PadRight(intSpace, ' '));

                //idx = _text.IndexOf(keyword, idx+keyword.Length+1, StringComparison.CurrentCultureIgnoreCase);

                idx += m.Index + 1 + m.Length + intSpace;

                if (idx > _text.Length - 1)
                {
                    break;
                }

                m = Regex.Match(_text.Substring(idx), keyword, RegexOptions.IgnoreCase);              

                ++cnt;
                if (cnt > 100)
                {
                    break;
                }
            }
        }
    }
}
