//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;
    
    public partial class TypeContact:Entity
    {
        public TypeContact()
        {
            this.Contacts = new HashSet<Contact>();
        }
    
        //public int TypeContactId { get; set; }
        public string LibelleTypeContact { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
    }
}