using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace WPFMediaKit
{
    public class DeviceParaInfo
    {
        public DeviceParaInfo(VideoProcAmpProperty videoProperty, string value, string deviceName)
        {
            VideoProperty = videoProperty;
            Value = value;
            DeviceName = deviceName;
        }
        private string _Value;
        public VideoProcAmpProperty VideoProperty { get;set; }
        public string Value { get; set; }
        public string DeviceName { get; set; }
    }
}
