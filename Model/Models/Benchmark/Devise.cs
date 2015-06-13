using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Traduction;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public class Devise : Entity
    {
        public string CodeDevise { get; set; }
        public string Libelle { get; set; }
        public string UrlImage { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual ICollection<DeviseTraduction> DeviseTraductions { get; set; }
    }
}
