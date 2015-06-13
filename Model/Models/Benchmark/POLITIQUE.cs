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

    public partial class Politique : Entity
    {
        public Politique()
        {
            this.Simulations = new HashSet<Simulation>();
        }
    
        //public int PolitiqueId { get; set; }
        public string LibellePolitique { get; set; }
        //public string DetaillePolitique { get; set; }
        public string Device { get; set; }
        public string Objectif { get; set; }
        public string Benefice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    
        public virtual ICollection<Simulation> Simulations { get; set; }
        public virtual ICollection<PolitiqueTraduction> PolitiqueTraductions { get; set; }
    }
}
