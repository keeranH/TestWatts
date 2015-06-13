using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.Models.Benchmark
{
    public class EquivalenceTraduction: Entity
    {
        public int EquivalenceId { get; set; }
        public int LangueId { get; set; }       
        public string LibelleEquivalence { get; set; }
        public int Valeur { get; set; }
        public string Mesure { get; set; }
        public bool Afficher { get; set; }
        public string LienImage { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual Equivalence Equivalence { get; set; }
        public virtual Langue Langue { get; set; }
    }
}
