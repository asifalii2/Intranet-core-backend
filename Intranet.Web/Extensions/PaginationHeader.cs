using Intranet.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace Intranet.Extensions
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }

    public static class PaginationExtension
    {
        /// <summary>
        /// Adds Pagination response header with CurrentPage, ItemsPerPage, TotalItems and TotalPages
        /// </summary>        
        public static void AddPaginationHeader<T>(this HttpResponse response, Core.DTOs.Paginations.IPagedList<T> pagedList)
        {
            var camelCaseFormatter = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var paginationHeader = new PaginationHeader(pagedList.CurrentPage, pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages);

            response.Headers.Add(Headers.PaginationHeaderKey, JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
        }
    }
}
