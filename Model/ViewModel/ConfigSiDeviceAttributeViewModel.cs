using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ConfigSiDeviceAttributeViewModel : EntityViewModel
    {
        public int IdDevice { get; set; }
        public int IdCategorie { get; set; }
        public decimal NbrJourTravaille { get; set; }
        public decimal NbrHeuresIntensifJour { get; set; }
        public decimal NbrHeuresOffJour { get; set; }
        public bool UtiliseEnergieExterne { get; set; }
        public decimal PourcentConsomExtern { get; set; }
        public decimal CoeffAbbatement { get; set; }
        public decimal NbrHeuresNonIntensifJour { get; set; }
        [Range(0,1)]
        public decimal CoeffNonIntensif { get; set; }
        [Range(0, 1)]
        public decimal? CoeffModeOff { get; set; }
        public int Quantite { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public Guid Guid { get; set; }
        public int? TypeUsageId { get; set; }
        public string URL { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string LibelleDevice { get; set; }

        public bool EstNouveau { get; set; }

        public ConfigSiConsommationViewModel ConfigSiConsommationViewModel { get; set; }

    }
}
