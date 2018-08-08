using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using DirectShowLib;
using Splash.IO.PORTS;

namespace EZUSB
{
    public partial class MyUsbWatcher
    {
        private DsDevice[] m_videoInputDevices;
        USB ezUSB = new USB();
        public MyUsbWatcher()
        {
        }
        private int iCameraCount = 0;
        public void AddWatcher()
        {
            //是要在USB变动之前判断已经有多少个摄像头
            iCameraCount = VideoInputDevices.Length;
            ezUSB.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 2));
        }

        public delegate void CameraInsert(DsDevice[] VideoInputDevices);
        public event CameraInsert eventCameraInsert;
        private void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                //USB插入
                //usb插入后判断摄像头是否插入的是摄像头
                //通过已有的usb判断？？？？
                if (GetDevices(FilterCategory.VideoInputDevice).Length > iCameraCount)
                {
                    m_videoInputDevices = GetDevices(FilterCategory.VideoInputDevice);
                    iCameraCount = m_videoInputDevices.Length;
                    eventCameraInsert(VideoInputDevices);
                }
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {   //USB拔出
                //如果是摄像头的拔出
                if (GetDevices(FilterCategory.VideoInputDevice).Length == iCameraCount - 1)
                {
                    iCameraCount--;
                }
            }
        }

        public void DelWatcher()
        {
            ezUSB.RemoveUSBEventWatcher();
        }
        ~MyUsbWatcher()
        {
            DelWatcher();
        }

        private DsDevice[] GetDevices(Guid filterCategory)
        {
            return (from d in DsDevice.GetDevicesOfCat(filterCategory)
                    select d).ToArray();
        }

        public DsDevice[] VideoInputDevices
        {
            get
            {
                if (m_videoInputDevices == null)
                {
                    m_videoInputDevices = GetDevices(FilterCategory.VideoInputDevice);
                }
                return m_videoInputDevices;
            }
        }
    }
}
