using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class AdresseViewModel
    {


        public int Id { get; set; }
        public int PaysId { get; set; }

        [Required]
        public string Adresse1 { get; set; }

        [Required]
        public string Adresse2 { get; set; }

        [Required]
        public string Adresse3 { get; set; }

        [Required]
        public string CodePostal { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Ville { get; set; }

        public virtual List<Pays> ListPays { get; set; }

        public virtual Pays Pays { get; set; }
    }
}
