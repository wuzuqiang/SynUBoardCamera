using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace WPFMediaKit.Model
{
    public class CameraControlRangeParameter
    {
        public CameraControlProperty CameraControlProperty { get; set; }
        public int MaxValue { get; set; }
        public int _MaxValue;
        public int MinValue { get; set; }
        public int DefaultValue { get; set; }
        public int SetpValue { get; set; }
        public CameraControlFlags CameraControlFlags { get; set; }
    }
}
