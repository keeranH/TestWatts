using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ListeEquivalenceViewModel: EntityViewModel
    {
        public List<Equivalence> Equivalences { get; set; }
        public int Total { get; set; }
    }
}
