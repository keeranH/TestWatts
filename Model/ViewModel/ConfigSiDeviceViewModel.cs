using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Enum;

namespace Econocom.Model.ViewModel
{
    public class ConfigSiDeviceViewModel

    {   
        public ConfigSiDeviceViewModel()
        {
            this.IsModified = false;
        }
        public int  Id { get; set; }
        public int TypeDeviceId { get; set; }
        public string LibelleDevice { get; set; }
        public string LibelleAffiche { get; set; }
        public string URL { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantite { get; set; }
        public bool IsModified { get; set; }
        public bool EstNouveau { get; set; }
        public bool EstParentModifie { get; set; }
        public int Source { get; set; }
        public bool EstRacineModifie { get; set; }
        public Guid Guid { get; set; }
        public int? TypeUsageId { get; set; }

        public ConfigSiDeviceAttributeViewModel detailsDeviceType { get; set; }
        public virtual List<GammesDeviceViewModel> Gammes { get; set; }


    }


}
