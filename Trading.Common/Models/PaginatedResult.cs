using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Common.Models
{
    public record class PaginatedResult<T> where T : class
    {
        public List<T> Results { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }

        public bool IsPrevAvailable {  get; set; }
        public bool IsNextAvailable {  get; set; }
    }
}
