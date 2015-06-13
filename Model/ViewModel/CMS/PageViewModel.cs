using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Econocom.Model.Models.CMS;

namespace Econocom.Model.ViewModel.CMS
{
    public class PageViewModel:EntityViewModel
    {
        [Required]
        public string Nom { get; set; }
        public string Lien { get; set; }
        public bool Actif { get; set; }
        public int? SectionId { get; set; }
        public Nullable<int> ModeleModereId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        //[CsvField(Ignore = true)]
        public virtual Modele Modele { get; set; }

        //[CsvField(Ignore = true)]
        public virtual ICollection<Modele> Modeles { get; set; } 
    }
}