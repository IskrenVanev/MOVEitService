using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOVEit.Platform.Models
{
    public class FolderItemDto
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string LastUpdateTime { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string FolderType { get; set; }
        public bool IsShared { get; set; }
    }

    public class PagingDto
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class SortingDto
    {
        public string SortField { get; set; }
        public string SortDirection { get; set; }
    }

    public class FolderResponseDto
    {
        public List<FolderItemDto> Items { get; set; }
        public PagingDto Paging { get; set; }
        public List<SortingDto> Sorting { get; set; }
    }
}
