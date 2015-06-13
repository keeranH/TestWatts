using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Econocom.Model.ViewModel
{
    public class UtilisateurSouscriptionViewModel
    {
        public List<Utilisateurs> ListeUtilisateurs { get; set; }
    }
    
    public class Utilisateurs
    {
        //[Required(ErrorMessage = "ErrorRequired")]
        public string Nom { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        public string Prenom { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        //[RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "ErrorMail")]       
        public string Email { get; set; }

        public int? ClientId { get; set; }

        public int? id { get; set; }

        public bool GenereMDP { get; set; }

    }

}


