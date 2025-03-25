using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MOVEit.Platform.Models
{
    public class Paging
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }

    public class FolderFilesResponseDTO
    {
        [JsonPropertyName("items")]
        public List<object> Items { get; set; }

        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("sorting")]
        public List<Sorting> Sorting { get; set; }
    }

    public class Sorting
    {
        [JsonPropertyName("sortField")]
        public string SortField { get; set; }

        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; }
    }
}
