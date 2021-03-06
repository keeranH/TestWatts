//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econocom.Model.Models.CMS
{
    using System;
    using System.Collections.Generic;

    public partial class Page : Entity
    {
        public Page()
        {
            this.ContenuModeres = new HashSet<ContenuModere>();
            //this.ContenuNonModeres = new HashSet<ContenuNonModere>();
            //this.HistoriqueContenus = new HashSet<HistoriqueContenu>();
            //this.HistoriqueModelePages = new HashSet<HistoriqueModelePage>();
            this.Routages = new HashSet<Routage>();
        }
    
        //public int PageId { get; set; }
        public string Lien{ get; set; }
        public bool Actif { get; set; }
        public string Nom { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> ModeleModereId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        //public Nullable<int> ModeleNonModereId { get; set; }
    
        public virtual ICollection<ContenuModere> ContenuModeres { get; set; }
        //public virtual ICollection<ContenuNonModere> ContenuNonModeres { get; set; }
        //public virtual ICollection<HistoriqueContenu> HistoriqueContenus { get; set; }
        //public virtual ICollection<HistoriqueModelePage> HistoriqueModelePages { get; set; }
        public virtual Modele ModeleModere { get; set; }
        //public virtual Modele ModeleNonModere { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Routage> Routages { get; set; }
    }
}
