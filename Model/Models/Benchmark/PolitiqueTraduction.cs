using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public class PolitiqueTraduction:Entity
    {
        public string PolitiqueId { get; set; }
        public string LangueId { get; set; }
        public string LibellePolitique { get; set; }
        public string DetaillePolitique { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
