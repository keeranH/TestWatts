using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Required]
        public string RaisonSociale { get; set; }

        [Required]
        public string Adresse1 { get; set; }

        [Required]
        public string Ville { get; set; }

        public string LibellePays { get; set; }

        public string CodePostal { get; set; }

        public string CodeInterneWattsGreen { get; set; }

        public Nullable<System.DateTime> DateDebut { get; set; }
       
        public Nullable<System.DateTime> DateValidation { get; set; }

        public string Tva { get; set; }

        public string Groupe { get; set; }

        public string CodeNaf { get; set; }

        public int ChiffresAffaires { get; set; }

        public string ClientEconocom { get; set; }

        public string VotreContacte { get; set; }

        public string LibelleTarif { get; set; }

        [Required]
        public int TypeClient { get; set; }

        public bool ConfigSi { get; set; }
    }
}
