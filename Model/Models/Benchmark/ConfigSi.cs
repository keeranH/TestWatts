using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public partial class ConfigSi:Entity
    {
        public ConfigSi()
        {
            this.Simulations = new HashSet<Simulation>();
            this.ConfigSiDevices = new HashSet<ConfigSiDevice>();
            this.Resultats = new HashSet<Resultat>();
            this.ConfigSiClients = new HashSet<ConfigSiClient>();
        }

        public string LibelleConfigSi { get; set; }
        public DateTime? DateModification { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int Statut { get; set; }

        public virtual ICollection<Simulation> Simulations { get; set; }
        public virtual ICollection<Resultat> Resultats { get; set; }
        public virtual ICollection<ConfigSiDevice> ConfigSiDevices { get; set; }
        public virtual ICollection<ConfigSiClient> ConfigSiClients { get; set; }

    }
}
