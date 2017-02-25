using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using models;

namespace api.Core.Utils
{
    public static class Helpers
    {
        public static int Compare(string x, string y)
        {
            if (x == y) return 0;
            var version = new { First = GetVersion(x), Second = GetVersion(y) };
            int limit = Math.Max(version.First.Length, version.Second.Length);
            for (int i = 0; i < limit; i++)
            {
                int first = version.First.ElementAtOrDefault(i);
                int second = version.Second.ElementAtOrDefault(i);
                if (first > second) return 1;
                if (second > first) return -1;
            }
            return 0;
        }

        public static int[] GetVersion(string version)
        {
            return (from part in version.Split('.')
                    select Parse(part)).ToArray();
        }

        public static int Parse(string version)
        {
            int result;
            int.TryParse(version, out result);
            return result;
        }

        public static IEnumerable<TSource> RemoveMarkersByVersion<TSource>(this IEnumerable<TSource> source, Dictionary<string, List<long>> keySelector)
        {
            var select = keySelector;
            var ctx = HttpContext.Current.Request;
            return (from element in source 
                    let markers = element as MarkerTypes 
                    let isBreak = keySelector
                    .Where(key => markers != null && key.Value.Contains(markers.MarkerTypeID))
                    .Any(key => Helpers.Compare(ctx.Headers["Version"].Trim(), key.Key) < 0) where !isBreak select element).ToList();
        }
    }
}