using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace api.Core
{
    public static class GenericJoin
    {
        public static string Join<T>(this IEnumerable<T> target, Func<T, object> valueSelector)
        {
            StringBuilder result = new StringBuilder();
            foreach (T item in target)
            {
                result.Append(valueSelector(item).ToString());
                result.Append(",");
            }
            result.Length--;  //remove the trailing comma 
               
            return result.ToString();
        }
    }
}