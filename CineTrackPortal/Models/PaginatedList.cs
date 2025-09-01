using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CineTrackPortal.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Returns a list of page numbers and nulls (for ellipses) for pagination bar rendering.
        /// </summary>
        public List<int?> GetPageBar(int maxPagesToShow = 7)
        {
            var pages = new List<int?>();
            if (TotalPages <= maxPagesToShow)
            {
                for (int i = 1; i <= TotalPages; i++)
                    pages.Add(i);
                return pages;
            }

            int numSidePages = 2; // Number of pages to show at the start and end
            int numAroundCurrent = 1; // Number of pages to show around the current page

            // Always show first page
            pages.Add(1);

            // Show page 2 if it's not adjacent to the first page
            if (PageIndex - numAroundCurrent > numSidePages + 1)
            {
                pages.Add(null); // Ellipsis
            }
            else if (TotalPages > maxPagesToShow)
            {
                for (int i = 2; i < Math.Max(PageIndex - numAroundCurrent, 2); i++)
                    pages.Add(i);
            }

            int left = Math.Max(PageIndex - numAroundCurrent, 2);
            int right = Math.Min(PageIndex + numAroundCurrent, TotalPages - 1);

            for (int i = left; i <= right; i++)
                pages.Add(i);

            // Show ellipsis if needed before the last pages
            if (PageIndex + numAroundCurrent < TotalPages - numSidePages)
            {
                pages.Add(null); // Ellipsis
            }
            else if (TotalPages > maxPagesToShow)
            {
                for (int i = right + 1; i < TotalPages; i++)
                    pages.Add(i);
            }

            // Always show last page
            if (TotalPages > 1)
                pages.Add(TotalPages);

            return pages;
        }
    }
}