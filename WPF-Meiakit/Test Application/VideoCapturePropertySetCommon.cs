using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using WPFMediaKit.DirectShow.Controls;

namespace WPFMediaKit
{
    public static class VideoCapturePropertySetCommon
    {
        /// <summary>
        /// 设置视频属性
        /// </summary>
        /// <param name="videoCaptureElement">保存视频的控件</param>
        /// <param name="videoProcAmpProperty">VideoProcAmpProperty枚举，亮度、对比度等等</param>
        /// <param name="iVal">枚举的值</param>
        public static void SetVideoProcAmpProperty(this VideoCaptureElement videoCaptureElement, VideoProcAmpProperty videoProcAmpProperty, int iVal)
        {
            videoCaptureElement.SetVideoProcAmpProperty(videoProcAmpProperty, iVal);
        }
        //public string ToString()
        //{
        //    return "";
        //}
    }
}
