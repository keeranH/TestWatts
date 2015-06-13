using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class GammesDeviceViewModel
    {
        public int IdDevice { get; set; }
        public string LibelleDevice { get; set; }
        public int IdClasse { get; set; }
        public string Libeller { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantiter { get; set; }
        public bool isModified { get; set; }
        public bool EstEnfantModifie { get; set; }
        public Guid Guid { get; set; }

        public List<AgeDeviceViewModel> AgeDeviceViewModels { get; set; }
    }
}
