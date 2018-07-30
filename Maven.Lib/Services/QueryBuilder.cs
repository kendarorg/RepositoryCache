using Ioc;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Services
{

    public class QueryBuilder : IQueryBuilder, ISingleton
    {
        public ParsedQuery ParseQuery(String q)
        {

            var pq = new ParsedQuery();
            if (string.IsNullOrWhiteSpace(q))
            {
                return pq;
            }

            var splitted = q.Split(' ');
            for (var i = 0; i < splitted.Length; i++)
            {
                var item = splitted[i];

                if (item[0] == '\"')
                {
                    var res = ParseComplexString(splitted, ref i).Trim('\"');
                    if (res.ToLowerInvariant() == "and" || res.ToLowerInvariant() == "or")
                    {
                        continue;
                    }
                    pq.FreeText.Add(res);
                }
                else if (GetKeyword(item) != null)
                {
                    ParseKeyword(pq, splitted, ref i, item);
                }
                else
                {
                    var res = item.Replace("+", " ");
                    if (res.ToLowerInvariant() == "and" || res.ToLowerInvariant() == "or")
                    {
                        continue;
                    }
                    pq.FreeText.Add(res);
                }
            }
            return pq;
        }

        private void ParseKeyword(ParsedQuery pq, string[] splitted, ref int i, string item)
        {
            var keyword = GetKeyword(item);
            var subItem = item.Substring(keyword.Length + 1);
            if (subItem[0] == '\"' && subItem[subItem.Length - 1] == '\"' && subItem.Length > 1 && subItem[subItem.Length - 2] != '\\')
            {
                pq.Keys.Add(keyword, subItem.Trim('\"'));
            }
            else if (subItem[0] != '\"')
            {
                pq.Keys.Add(keyword, subItem.Replace("+", " "));
            }
            else
            {
                var complexItem = ParseComplexString(splitted, ref i);
                subItem = complexItem.Substring(keyword.Length + 1);
                pq.Keys.Add(keyword, subItem.Trim('\"').Replace("+", " "));
            }
        }

        private string GetKeyword(string item)
        {
            var litem = item.ToLowerInvariant();
            if (litem.StartsWith("g:")) return "group";
            if (litem.StartsWith("v:")) return "version";
            if (litem.StartsWith("p:")) return "packaging";//jar war ear bundle
            if (litem.StartsWith("l:"))
            {
                //return "classfier";
                throw new NotSupportedException();
            }
            if (litem.StartsWith("a:")) return "packageid";//packageid
            if (litem.StartsWith("c:")) return "class";
            if (litem.StartsWith("1:")) return "sha1";
            if (litem.StartsWith("tags:")) return "tags";
            if (litem.StartsWith("timestamp:")) return "timestamp";
            return null;
        }

        private static string ParseComplexString(string[] splitted, ref int i)
        {
            var tempItem = "";
            for (; i < splitted.Length; i++)
            {
                var subItem = splitted[i];
                if (subItem[subItem.Length - 1] == '\"' && subItem.Length > 1 && subItem[subItem.Length - 2] != '\\')
                {
                    if (tempItem.Length == 0)
                    {
                        return subItem;
                    }
                    else
                    {
                        return tempItem + " " + subItem;
                    }
                }
                else
                {
                    if (tempItem.Length == 0)
                    {
                        tempItem = subItem;
                    }
                    else
                    {
                        tempItem += " " + subItem;
                    }
                }
            }
            return tempItem;
        }
    }
}
