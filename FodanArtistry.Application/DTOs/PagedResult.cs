using System.Collections.Generic;

namespace FodanArtistry.Application.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
    }
}