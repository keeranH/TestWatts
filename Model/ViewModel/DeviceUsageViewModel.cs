using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    class DeviceUsageViewModel
    {
        public string deviceName;
        public string deviceQuantity;
        public string deviceFamilyId;
        public string deviceCategoryId;
        public List<DeviceAtributeViewModel> deviceAtributes;
    }
}
