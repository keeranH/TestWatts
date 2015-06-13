using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.Models.Traduction
{
    public class TypeRatioTraduction : Entity
    {
        public int TypeRatioId { get; set; }
        public int LangueId { get; set; }  

        public string LibelleTypeRatio { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual TypeRatio TypeRatio { get; set; }
        public virtual Langue Langue { get; set; }
    }
}
