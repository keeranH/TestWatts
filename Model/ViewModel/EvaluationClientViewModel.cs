using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;


namespace Econocom.Model.ViewModel
{
    public class EvaluationClientViewModel
    {
        //[Required(ErrorMessage = "ErrorRequired")]
        public string RaisonSociale { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string Groupe { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string Nom { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string Prenom { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string Fonction { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "ErrorMail")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string NumeroPhone { get; set; }

        public Nullable<int> PaysId { get; set; }

        public virtual Pays Pays { get; set; }

        public virtual List<Pays> ListePays { get; set; }
    }
}
