using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ContactViewModel:EntityViewModel
    {       
        //[CsvField(Ignore = true)]
        public Nullable<int> TypeContactId { get; set; }

         //[CsvField(Ignore = true)] 
        public int ClientId { get; set; }

         //[CsvField(Ignore = true)]
        public Nullable<int> ReponseId { get; set; }

        [Required]
        //[CsvField(Name = "Nom")]
        public string NomContact { get; set; }
        
        [Required]
        public string PrenomContact { get; set; }
        
        [Required]
        public string Fonction { get; set; }
        
        [Required]
        //[CsvField(Ignore = true)]
        public string NumeroPhone { get; set; }
        
        [Required]
        //[CsvField(Ignore = true)]
        public string NumeroGSM { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        //[CsvField(Ignore = true)]
        public string MotPasse { get; set; }

         //[CsvField(Ignore = true)]
        public Nullable<int> Authorisations { get; set; }

         //[CsvField(Ignore = true)]
        public string CodeVerification { get; set; }

        public String Reponse { get; set; }

        public String Question { get; set; }

        public String Profil { get; set; }

        public DateTime? DatePremiereConnexion { get; set; }

        public string CodeInterneWattsGreen { get; set; }

    }
}
