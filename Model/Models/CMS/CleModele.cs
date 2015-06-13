//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Econocom.Model;

namespace Econocom.Model.Models.CMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class CleModele:Entity
    {
        public CleModele()
        {
            this.ContenuModeres = new HashSet<ContenuModere>();
            //this.ContenuNonModeres = new HashSet<ContenuNonModere>();
            //this.HistoriqueContenus = new HashSet<HistoriqueContenu>();
        }

        //public int CleModeleId { get; set; }
        public string Nom { get; set; }
        public Nullable<int> ModeleId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    
        public virtual Modele Modele { get; set; }
        public virtual ICollection<ContenuModere> ContenuModeres { get; set; }
        //public virtual ICollection<ContenuNonModere> ContenuNonModeres { get; set; }
        //public virtual ICollection<HistoriqueContenu> HistoriqueContenus { get; set; }
    }
}