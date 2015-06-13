using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class SimulationViewModel
    {
        public Nullable<int> PolitiqueId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> OrigineId { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public DateTime DateDebut { get; set; }
        public Nullable<DateTime> DateFin { get; set; }
        public Nullable<DateTime> DateModification { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string LibelleSimulation { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Origine Origine { get; set; }
        public virtual Politique Politique { get; set; }
        public virtual ConfigSi ConfigSi { get; set; }

        public virtual List<Politique> Politiques { get; set; }
    }
}
