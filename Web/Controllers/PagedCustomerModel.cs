using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Web.Controllers
{
    public class PagedCustomerModel
    {
        public TYPEOBJET TypeObjet { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }
    }
}
