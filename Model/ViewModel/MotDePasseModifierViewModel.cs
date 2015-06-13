using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class MotDePasseModifierViewModel
    {
        public virtual Contact Contact { get; set; }

        public virtual Question Question { get; set; }

        public virtual Reponse Reponse { get; set; }

        public virtual List<Question> Questions { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string ReponseSaisie { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string AncientMotDePasse { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string NouveauMotDePasse { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string ConfirmerMotDePasse { get; set; }

        public int IdContact { get; set; }

        public int IdQuestion { get; set; }
    }
}

