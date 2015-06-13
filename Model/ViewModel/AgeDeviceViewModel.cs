using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class AgeDeviceViewModel:EntityViewModel
    {
        public int SecteurActiviteId { get; set; }
        public int IdDevice { get; set; }
        public int IdAge { get; set; }
        public int IdClasse { get; set; }
        public string LibelleDevice { get; set; }
        [Required]
        public string LibelleAgeDevice { get; set; }
        public int ClientId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantite { get; set; }
        public Guid UniqueGuid { get; set; }

        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }    
    }
}
