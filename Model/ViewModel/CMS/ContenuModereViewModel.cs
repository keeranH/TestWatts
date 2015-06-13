using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.ViewModel.CMS
{
    public class ContenuModereViewModel:EntityViewModel
    {
         [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Contenu { get; set; }
        
        public int? PageId { get; set; }
        public int? CleModeleId { get; set; }
        public int? LangueId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual CleModele CleModele { get; set; }
        public virtual Langue Langue { get; set; }
        public virtual Page Page { get; set; }

        public virtual List<Langue> ListeLangues { get; set; }
    }
}
