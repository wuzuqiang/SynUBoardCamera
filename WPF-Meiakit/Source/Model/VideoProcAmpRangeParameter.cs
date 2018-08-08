using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace WPFMediaKit.Model
{
    public class VideoProcAmpRangeParameter
    {
        public VideoProcAmpProperty VideoProcAmpProperty { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int DefaultValue { get; set; }
        public int SetpValue { get; set; }
        public VideoProcAmpFlags VideoProcAmpFlags { get; set; }
    }
}
