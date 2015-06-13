using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Econocom.Model.Models.CMS;

namespace Econocom.Model.ViewModel
{
    public class ContenuMailViewModel: EntityViewModel
    {
        public int TypeMailId { get; set; }
        public int LangueId { get; set; }

        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Contenu { get; set; }
        public string Sujet { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        // [CsvField(Ignore = true)]
        public virtual List<Langue> ListeLangues { get; set; }

        // [CsvField(Ignore = true)]
        public virtual Langue Langue { get; set; }
    }
}
