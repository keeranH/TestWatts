using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel.Report
{
    public class RapportViewModel
    {
        public int Total { get; set; }
        public List<Rapport> Rapports { get; set; }
    }
}
