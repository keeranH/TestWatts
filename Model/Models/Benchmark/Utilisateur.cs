using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public class Utilisateur: Entity
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public int Authorization { get; set; }

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
