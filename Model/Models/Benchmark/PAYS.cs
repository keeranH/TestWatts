//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Traduction;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class Pays : Entity
    {
        public Pays()
        {
            this.Adresses = new HashSet<Adresse>();            
        }
    
        //public int PaysId { get; set; }
        public string LibellePays { get; set; }
        public decimal? CoefficientConversion { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public virtual ICollection<Adresse> Adresses { get; set; }       
        public virtual ICollection<Langue> Langues { get; set; }
        public virtual ICollection<PaysTraduction> PaysTraductions { get; set; }
    }
}