using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ClasseDeviceViewModel:EntityViewModel
    {
        public string LibelleClasseDevice { get; set; }
        public int SecteurActiviteId { get; set; }
        public List<AgeDeviceConsoViewModel> AgeDeviceConsoViewModels { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }   
    }
}
