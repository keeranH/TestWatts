using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class CalculateurViewModel
    {
        public virtual List<FamilleDeviceViewModel> FamilleDeviceViewModels { get; set; }
        public int Effectif { get; set; }
        public decimal PrixKWatt { get; set; }
        public decimal? CoefficientConversion { get; set; }
        public int? SuperficieDataCenter { get; set; }
        public decimal TauxEquipement { get; set; }
        public int NombreSite { get; set; }

        public ConfigSi ConfigSi { get; set; }
        public ConfigSiClient ConfigSiClient { get; set; }

        //en sortie
        public decimal? TotalConsoWatt { get; set; }
        public decimal? TotalConsoCo2 { get; set; }
        public decimal? TotalPrix { get; set; }
    }
}
