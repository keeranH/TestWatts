using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class EvaluationViewModel
    {        
        public string RaisonSociale { get; set; }

        public string Groupe { get; set; }

        public string Nom { get; set; }

        public string Prenom { get; set; }

        public string Fonction { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "ErrorMail")]
        public string Email { get; set; }

        public string NumeroPhone { get; set; }

        public Nullable<int> PaysId { get; set; }

        public virtual Pays Pays { get; set; }

        public virtual List<Pays> ListePays { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public int Effectifs { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public Decimal TauxEquipement { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public int NombreSites { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public bool Accepter { get; set; }

        public Decimal PrixMoyenKwatt { get; set; }

        public Nullable<int> SecteurActiviteId { get; set; }
        public virtual SecteurActivite SecteurActivite { get; set; }
        public virtual List<SecteurActivite> ListeSecteurActivite { get; set; }

        public Nullable<int> DeviseId { get; set; }
        public virtual Devise Devise { get; set; }
        public virtual List<Devise> ListeDevise { get; set; }
    }
}
