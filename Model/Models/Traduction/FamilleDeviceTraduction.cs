//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.Models.Traduction
{
    using System;
    using System.Collections.Generic;

    public partial class FamilleDeviceTraduction : Entity
    {
        public int FamilleDeviceId { get; set; }
        public int LangueId { get; set; }        
        public string LibelleFamilleDevice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual FamilleDevice FamilleDevice { get; set; }
        public virtual Langue Langue { get; set; }
    }
}
