using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ListeDeviseViewModel : EntityViewModel
    {
        public List<Devise> Devises { get; set; }
        public int Total { get; set; }
    }
}
