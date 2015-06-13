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

    public partial class ClasseDevice : Entity
    {
        public ClasseDevice()
        {
            this.ConsoWattHeurs = new HashSet<ConsoWattHeur>();
            this.DeviceClients = new HashSet<DeviceClient>();
            //this.Usages = new HashSet<Usage>();
            this.VentilationClasseAgeDevices = new HashSet<VentilationClasseAgeDevice>();
            this.VentilationClasseDevices = new HashSet<VentilationClasseDevice>();
            this.ConsommationDeviceClients = new HashSet<ConsommationDeviceClient>();
        }
    
        //public int ClasseDeviceId { get; set; }
        public Nullable<int> SecteurActiviteId { get; set; }
        public string LibelleClasseDevice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    
        public virtual SecteurActivite SecteurActivite { get; set; }
        public virtual ICollection<ConsoWattHeur> ConsoWattHeurs { get; set; }
        public virtual ICollection<DeviceClient> DeviceClients { get; set; }
        //public virtual ICollection<Usage> Usages { get; set; }
        public virtual ICollection<VentilationClasseAgeDevice> VentilationClasseAgeDevices { get; set; }
        public virtual ICollection<VentilationClasseDevice> VentilationClasseDevices { get; set; }
        public virtual ICollection<ConsommationDeviceClient> ConsommationDeviceClients { get; set; }
        public virtual ICollection<ClasseDeviceTraduction> ClasseDeviceTraductions { get; set; }
    }
}
