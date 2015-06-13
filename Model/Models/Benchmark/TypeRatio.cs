using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Traduction;

namespace Econocom.Model.Models.Benchmark
{
    public class TypeRatio : Entity
    {
        public string LibelleTypeRatio { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual ICollection<TypeRatioTraduction> TypeRatioTraductions { get; set; }
    }
}
