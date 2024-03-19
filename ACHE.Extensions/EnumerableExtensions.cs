using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace ACHE.Extensions
{
    public static class EnumerableExtensions
    {

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortExpression, string sortType, IComparer<object> comparer)
        {
            sortExpression += "";
            string[] parts = sortExpression.Split(' ');
            bool descending = false;

            if (sortType.ToLower() == "desc")
                descending = true;

            if (parts.Length > 0 && parts[0] != "")
            {
                if (descending)
                    return list.OrderByDescending(x => x, comparer);
                else
                    return list.OrderBy(x => x, comparer);
            }

            return list;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortExpression, string sortType)
        {
            sortExpression += "";
            string[] parts = sortExpression.Split(' ');
            string property = "";
            bool descending = false;

            if (sortType.ToLower() == "desc")
                descending = true;

            if (parts.Length > 0 && parts[0] != "")
            {
                property = parts[0];

                PropertyInfo prop = typeof(T).GetProperty(property);
                if (prop != null)
                {
                    if (descending)
                        return list.OrderByDescending(x => prop.GetValue(x, null));
                    else
                        return list.OrderBy(x => prop.GetValue(x, null));
                }
            }

            return list;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortExpression, bool descending)
        {
            sortExpression += "";
            string[] parts = sortExpression.Split(' ');
            string property = "";


            if (parts.Length > 0 && parts[0] != "")
            {
                property = parts[0];

                PropertyInfo prop = typeof(T).GetProperty(property);
                if (prop != null)
                {
                    if (descending)
                        return list.OrderByDescending(x => prop.GetValue(x, null));
                    else
                        return list.OrderBy(x => prop.GetValue(x, null));
                }
            }

            return list;
        }
    }
}
