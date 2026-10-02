using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intranet.Core.DTOs.Paginations
{
    public class Pagination
    {
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }
}
