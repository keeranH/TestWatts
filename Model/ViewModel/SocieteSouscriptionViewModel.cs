using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class SocieteSouscriptionViewModel
    {
        public int? ClientId { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string RaisonSociale { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]       
        public string Addresse { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string CodePostal { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string Ville { get; set; }

        public string Tva { get; set; }
        
        public bool ClientEconocom { get; set; }

        public string LibelleClientEconocom { get; set; }

        public string Groupe { get; set; }

        public Nullable<int> PaysId { get; set; }

        public virtual Pays Pays { get; set; }

        public virtual List<Pays> ListPays { get; set; }

        public string LibellePays { get; set; }

        public string IdentificationNational { get; set; }

        public int? ChiffresAffaires { get; set; }

        public string VotreContacte { get; set; }

        public string CodeInterneWattsGreen { get; set; }

    }
}
