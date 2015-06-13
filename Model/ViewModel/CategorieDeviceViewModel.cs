using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class CategorieDeviceViewModel : EntityViewModel
    {
        public int CategorieDeviceId { get; set; }
        public int? FamilleDeviceId { get; set; }
        public string LibelleCategorieDevice { get; set; }
        public string URL { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public bool IsModified { get; set; }
        public int ClientId { get; set; }

        public virtual List<TypeDeviceViewModel> TypeDevices { get; set; }

        //en sortie
        public decimal? ConsoWatt { get; set; }
        public decimal? ConsoCo2 { get; set; }
        public decimal? Prix { get; set; }
        public decimal Pourcentage { get; set; }
    }
}
