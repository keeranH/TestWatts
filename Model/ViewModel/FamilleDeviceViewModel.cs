using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class FamilleDeviceViewModel : EntityViewModel
    {
        public int? SecteurActiviteId { get; set; }
        public string LibelleFamilleDevice { get; set; }
        public string URL { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public virtual List<CategorieDeviceViewModel> CategorieDevices { get; set; }
        public int? ClientId { get; set; }

        //en sortie
        public decimal? ConsoWatt { get; set; }
        public decimal? ConsoCo2 { get; set; }
        public decimal? Prix { get; set; }
        public decimal Pourcentage { get; set; }
        public bool IsModified { get; set; }
    }
}
