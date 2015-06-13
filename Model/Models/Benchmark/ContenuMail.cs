using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public class ContenuMail : Entity
    {
        public int TypeMailId { get; set; }
        public int LangueId { get; set; }
        public string Contenu { get; set; }
        public string Sujet { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual TypeMail TypeMail { get; set; }
    }
}
