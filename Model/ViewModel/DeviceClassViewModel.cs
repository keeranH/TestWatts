﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class DeviceClassViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public List<DeviceAgeViewModel> ViewModelDeviceAges { get; set; }

        public float Ratio { get; set; }
        public int Value { get; set; }
        public UsageViewModel ViewModelUsage { get; set; }
    }
}
