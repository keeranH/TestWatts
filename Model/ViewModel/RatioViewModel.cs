using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class RatioViewModel:EntityViewModel
    {
        public int SecteurActiviteId { get; set; }
        public int TypeDeviceId { get; set; }
        public int TypeRatioId { get; set; }
        public string LibelleRatio { get; set; }
        //public decimal RatioNbreEquipement { get; set; }
        //public decimal RatioNbreSite { get; set; }
        //public decimal RatioCA { get; set; }
        //public decimal RatioSuperficie { get; set; }
        //public decimal RatioSuperficieDataCenter { get; set; }
        public decimal? Valeur { get; set; }
        public System.DateTime DateDebut { get; set; }
        public Nullable<System.DateTime> DateFin { get; set; }
        public Nullable<System.DateTime> DateModification { get; set; }

        public TypeRatioViewModel TypeRatioViewModel { get; set; }
    }
}
