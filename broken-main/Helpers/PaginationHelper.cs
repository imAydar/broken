using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BrokenCode.Helpers
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Returns paginated collection of elements. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> Paginate<TResult>(this IEnumerable<TResult> source, int pageSize, int pageNumber)
        {
            return source?
                .Skip(pageSize * pageNumber)
                .Take(pageSize);
        }
    }
}