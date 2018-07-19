using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetV3.Services
{

    public class QueryBuilder : IQueryBuilder
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
                    pq.FreeText.Add(ParseComplexString(splitted, ref i).Trim('\"'));
                }
                else if (GetKeyword(item) != null)
                {
                    ParseKeyword(pq, splitted, ref i, item);
                }
                else
                {
                    pq.FreeText.Add(item.Replace("+"," "));
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
                pq.Keys.Add(keyword, subItem.Replace("+"," "));
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
            if (litem.StartsWith("id:")) return "id";
            if (litem.StartsWith("packageid:")) return "packageid";
            if (litem.StartsWith("title:")) return "title";
            if (litem.StartsWith("tags:")) return "tags";
            if (litem.StartsWith("author:")) return "author";
            if (litem.StartsWith("description:")) return "description";
            if (litem.StartsWith("summary:")) return "summary";
            if (litem.StartsWith("owner:")) return "owner";
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
