using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Econocom.Model.ViewModel
{
    public class SecteurActiviteViewModel: EntityViewModel
    {
        [Required]
        public string LibelleSecteurActivite { get; set; }
        public string SegmentActivite { get; set; }
        public string TypeActivite { get; set; }
        [Required]
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
