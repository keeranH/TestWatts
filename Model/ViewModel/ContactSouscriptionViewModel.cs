using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Econocom.Model.ViewModel
{
    public class ContactSouscriptionViewModel
    {
        [Required(ErrorMessage = "ErrorRequired")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "ErrorMail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string Fonction { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string Telephone { get; set; }

        public int? ClientId { get; set; }

        //public List<ContactSouscriptionViewModel> ListContact { get; set; }
    }

}


