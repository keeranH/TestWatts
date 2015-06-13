using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class DeviceCategoryViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int Value { get; set; }
        public List<ConfigSiDeviceViewModel> ViewModelDeviceTypes { get; set; }
    }
}
