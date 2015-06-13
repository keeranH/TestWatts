using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    class DeviceAtributeViewModel
    {

        public Nullable<int> IDAGEDEVICE { get; set; }

        public Nullable<int> IDCLASSEDEVICE { get; set; }

        public Nullable<int> IDTYPEUSAGE { get; set; }

        public Nullable<int> IDCLIENT { get; set; }

        public Nullable<int> IDTYPEDEVICE { get; set; }

        public int NBRJOURTRAVAILLE { get; set; }
        public decimal NBRHEURESINTENSIFJOUR { get; set; }
        public decimal NBRHEURESOFFJOUR { get; set; }
        public decimal COEFFNONINTENSIF { get; set; }
        public Nullable<decimal> COEFFMODEOFF { get; set; }
        public int QUANTITE { get; set; }
        public int DATEDEBUT { get; set; }
        public Nullable<int> DATEFIN { get; set; }
    }
}
