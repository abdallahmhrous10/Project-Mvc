using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Models
{
    public sealed class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        private PaginatedList(IEnumerable<T> items,int count, int pageindex,int pagesize): base(items)
        {
            PageIndex = pageindex;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count /(double) pagesize);
        }

        public static PaginatedList<T> Create(IEnumerable<T>source, int pageindex,int pagesize )
        {
            pageindex = Math.Max(pageindex,1);
            pagesize = Math.Max(pagesize,1);

            var Items  = source.ToList();

            return new PaginatedList<T>(Items.Skip((pageindex - 1) * pagesize).Take(pagesize),Items.Count, pageindex, pagesize);

        }


        public static PaginatedList<T> Create(
            IEnumerable<T> items,
            int totalCount,
            int pageIndex,
            int pageSize)
        {
            return new PaginatedList<T>(
                items,
                totalCount,
                pageIndex,
                pageSize);
        }

    }
}
