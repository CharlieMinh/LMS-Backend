using System;
using System.Collections.Generic;

namespace LMS.Application.DTOs
{
    public class PagedRequest
    {
        private const int MaxPageSize = 100;

        public int PageNumber { get; set; } = 1;
        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? 20 : value);
        }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
        public string? Search { get; set; }
    }

    public class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
