using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class VentilationClasseDeviceViewModel:EntityViewModel
    {
        public int SecteurActiviteId { get; set; }
        public int TypeDeviceId { get; set; }
        public int ClasseDeviceId { get; set; }
        public decimal Coefficient { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public int Quantite { get; set; }

        public virtual ClasseDevice ClasseDevice { get; set; }
        public virtual SecteurActivite SecteurActivite { get; set; }
        public virtual TypeDevice TypeDevice { get; set; }
    }
}
