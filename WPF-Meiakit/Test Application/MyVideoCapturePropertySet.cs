using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using WPFMediaKit.DirectShow.Controls;

namespace WPFMediaKit
{
    public static class MyVideoCapturePropertySet
    {
        /// <summary>
        /// 亮度设置
        /// </summary>
        /// <param name="captureElement"></param>
        /// <param name="iVal"></param>
        public static void SetBrightness(this VideoCaptureElement captureElement, int iVal)
        {
            captureElement.SetVideoProcAmpProperty(VideoProcAmpProperty.Brightness ,iVal);
        }
        /// <summary>
        /// 对比设置
        /// </summary>
        /// <param name="captureElement"></param>
        /// <param name="iVal"></param>
        public static void SetContrast(this VideoCaptureElement captureElement, int iVal)
        {
            captureElement.SetVideoProcAmpProperty(VideoProcAmpProperty.Contrast, iVal);
        }
        /// <summary>
        /// 饱和度设置
        /// </summary>
        /// <param name="captureElement"></param>
        /// <param name="iVal"></param>
        public static void SetHue(this VideoCaptureElement captureElement, int iVal)
        {
            captureElement.SetVideoProcAmpProperty(VideoProcAmpProperty.Hue, iVal);
        }
    }
}
