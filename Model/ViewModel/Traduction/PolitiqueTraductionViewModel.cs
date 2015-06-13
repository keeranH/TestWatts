using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;

namespace Econocom.Model.ViewModel.Traduction
{
    public class PolitiqueTraductionViewModel:EntityViewModel
    {
        public int PolitiqueId { get; set; }
        public int LangueId { get; set; }
        public string LibellePolitique { get; set; }
        //[UIHint("tinymce_jquery_full"), AllowHtml]
        public string Device { get; set; }
        //[UIHint("tinymce_jquery_full"), AllowHtml]
        public string Objectif { get; set; }
        //[UIHint("tinymce_jquery_full"), AllowHtml]
        public string Benefice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual Politique Politique { get; set; }
        public virtual Langue Langue { get; set; }

        public virtual List<Langue> ListeLangues { get; set; }
    }
}
