//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Econocom.Model.Models.Traduction;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class Equivalence : Entity
    {
        //public int EquivalenceId { get; set; }
        public string LibelleEquivalence { get; set; }
        public double Valeur { get; set; }
        public string Mesure { get; set; }
        public bool Afficher { get; set; }
        public string LienImage { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual ICollection<EquivalenceTraduction> EquivalenceTraductions { get; set; }
    }
}