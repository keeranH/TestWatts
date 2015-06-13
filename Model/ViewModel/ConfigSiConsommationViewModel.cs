using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ConfigSiConsommationViewModel
    {
       public int IdDevice { get; set; }
       public int ConfigSiDeviceId { get; set; }
       
       public  List<ClasseDeviceViewModel> ClasseDeviceViewModel { get; set; }
    }
}
