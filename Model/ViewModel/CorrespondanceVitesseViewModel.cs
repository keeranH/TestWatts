//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Web;

namespace Econocom.Model.ViewModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class CorrespondanceVitesseViewModel : EntityViewModel
    {
        public int TypeSocieteId { get; set; }
        public int TypeDeviceId { get; set; }
        public decimal Vitesse1 { get; set; }
        public decimal Vitesse2 { get; set; }
        public decimal Vitesse3 { get; set; }

        public int Vitesse1ClasseDeviceId { get; set; }
        public int Vitesse2ClasseDeviceId { get; set; }
        public int Vitesse3ClasseDeviceId { get; set; }
        
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
