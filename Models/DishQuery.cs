using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Models
{
    public class DishQuery
    {
        public string SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
