using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;

namespace Econocom.Model.ViewModel.Traduction
{
    public class EquivalenceTraductionViewModel: EntityViewModel
    {
        public int EquivalenceId { get; set; }
        public int LangueId { get; set; }       
        public string LibelleEquivalence { get; set; }       
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual Equivalence Equivalence { get; set; }
        public virtual Langue Langue { get; set; }
        public virtual List<Langue> ListeLangues { get; set; }
    }
}
