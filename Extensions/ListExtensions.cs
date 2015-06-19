using System;
using System.Collections.Generic;
using System.Linq;

namespace Wimt.CachingFramework.Extensions
{
    internal static class ListExtensions
    {
        public static string JoinItems(this IEnumerable<string> collection, string begin, string end, string separator)
        {
            if (collection.Any())
            {
                return String.Format(
                    "{0}{1}{2}",
                    begin,
                    String.Join(separator, collection.Select(x => x.ToString())),
                    end
                );
            }
            else
            {
                return String.Empty;
            }
        }
    }
}