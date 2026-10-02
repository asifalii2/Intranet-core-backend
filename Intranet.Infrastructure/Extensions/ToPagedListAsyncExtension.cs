using Intranet.Core.DTOs.Paginations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Extensions
{
    public static class ToPagedListAsyncExtension
    {
        /// <summary>
        /// Returns subset list of T with meta-data regarding pagination
        /// </summary>        
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, Pagination pagination)
        {
            var countQuery = await source.CountAsync();
            var count = countQuery;
            var itemsQuery = source.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize);
            var items = await itemsQuery.ToListAsync();
            return new PagedList<T>(items, count, pagination.Page, pagination.PageSize);
        }
    }
}
