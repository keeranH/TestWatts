using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public partial class ConfigSiConsommation:Entity
    {
       
        public Nullable<int> ConfigSiDeviceId { get; set; }
        public int ClasseDeviceId { get; set; }
        public int AgeDeviceId { get; set; }
        public decimal ConsommationIntensif { get; set; }
        public decimal ConsommationNonIntensif { get; set; }
        public int Quantite { get; set; }
        public DateTime DateModification{ get; set; }
        public Nullable<decimal> ConsommationWatt { get; set; }
        public Nullable<decimal> ConsommationCo2 { get; set; }
        public Nullable<decimal> CoefficientConversion { get; set; }
        public decimal ConsommationIntensifUnitaire { get; set; }
        public bool IsModified { get; set; }
        public virtual ClasseDevice ClasseDevice { get; set; }
        public virtual AgeDevice AgeDevice{ get; set; }
        public virtual ConfigSiDevice ConfigSiDevice { get; set; }

    }
}
