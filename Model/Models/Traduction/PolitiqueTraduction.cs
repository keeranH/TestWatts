using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.Models.Traduction
{
    public class PolitiqueTraduction:Entity
    {
        public int PolitiqueId { get; set; }
        public int LangueId { get; set; }
        public string LibellePolitique { get; set; }
        public string Device { get; set; }
        public string Objectif { get; set; }
        public string Benefice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual Politique Politique { get; set; }
        public virtual Langue Langue { get; set; }
    }
}
