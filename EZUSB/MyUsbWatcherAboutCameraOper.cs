using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Splash.IO.PORTS;

namespace EZUSB
{
    public class MyUsbWatcherAboutCameraOper
    {
        USB ezUSB = new USB();
        public MyUsbWatcherAboutCameraOper()
        {
            //Set("Win32_PnPEntity");
        }
        private int iCameraCount = 0;
        public void AddWatcher()
        {
            //是要在USB变动之前判断已经有多少个摄像头

            ezUSB.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 2));
        }

        public delegate void CameraInsert();
        public event CameraInsert eventCameraInsert;
        private void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                //USB插入
                //usb插入后判断摄像头是否插入的是摄像头
                USBChanged(e);
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {   //USB拔出
                //如果是摄像头的拔出
                USBChanged(e);
            }
        }
        public void USBChanged(EventArrivedEventArgs e)
        {
            //PnPEntityInfo Element = GetCameraEntity(e.NewEvent as ManagementObject);
            USB.WhoUSBControllerDevice(e);
            //ManagementObject mbo = e.NewEvent["TargetInstance"] as ManagementObject;    //will get the mbo value:null
            //PnPEntityInfo Element = GetCameraEntity(mbo);
            GetManagementBaseObjectProperties(e.NewEvent);
        }

        public void DelWatcher()
        {
            ezUSB.RemoveUSBEventWatcher();
        }
        ~MyUsbWatcherAboutCameraOper()
        {
            DelWatcher();
        }
        /// <summary>
        /// 即插即用设备信息结构
        /// </summary>
        public struct PnPEntityInfo
        {
            public String PNPDeviceID;      // 设备ID
            public String Name;             // 设备名称
            public String Description;      // 设备描述
            public String Service;          // 服务
            public String Status;           // 设备状态
            public UInt16 VendorID;         // 供应商标识
            public UInt16 ProductID;        // 产品编号 
            public Guid ClassGuid;          // 设备安装类GUID
        }
        public List<PnPEntityInfo> GetCameraList()
        {
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();
            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PnPClass='Camera'").Get();
            if (PnPEntityCollection != null)
            {
                foreach (ManagementObject Entity in PnPEntityCollection)
                {
                    Guid theClassGuid = new Guid(Entity["ClassGuid"] as String);    // 设备安装类GUID
                    PnPEntityInfo Element;
                    Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // 设备ID
                    Element.Name = Entity["Name"] as String;                // 设备名称
                    Element.Description = Entity["Description"] as String;  // 设备描述
                    Element.Service = Entity["Service"] as String;          // 服务
                    Element.Status = Entity["Status"] as String;            // 设备状态
                    Element.VendorID = 23;     // 供应商标识
                    Element.ProductID = 23;   // 产品编号
                    Element.ClassGuid = theClassGuid;   // 设备安装类GUID

                    UsbDevices.Add(Element);
                }
            }
            return UsbDevices;
        }
        public PnPEntityInfo GetCameraEntity(ManagementObject Entity)
        {
            Guid theClassGuid = new Guid(Entity["ClassGuid"] as String);    // 设备安装类GUID
            PnPEntityInfo Element;
            Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // 设备ID
            Element.Name = Entity["Name"] as String;                // 设备名称
            Element.Description = Entity["Description"] as String;  // 设备描述
            Element.Service = Entity["Service"] as String;          // 服务
            Element.Status = Entity["Status"] as String;            // 设备状态
            Element.VendorID = 23;     // 供应商标识
            Element.ProductID = 23;   // 产品编号
            Element.ClassGuid = theClassGuid;   // 设备安装类GUID
            return Element;

        }

        public void Set(string ManagementType)
        {
            StringBuilder sb = new StringBuilder();
            ManagementClass processClass = new ManagementClass(ManagementType);
            processClass.Options.UseAmendedQualifiers = true;

            PropertyDataCollection properties = processClass.Properties;

            sb.AppendLine("Win32_PnPEntity Property Names:");
            foreach (PropertyData property in properties)
            {
                sb.AppendLine(property.Name);
                foreach (QualifierData q in property.Qualifiers)
                {
                    if (q.Name.Equals("Description"))
                    {
                        sb.AppendLine(
                            processClass.GetPropertyQualifierValue(
                                property.Name, q.Name).ToString());
                    }
                }
            }
        }

        public void GetManagementBaseObjectProperties(ManagementBaseObject managementBaseObject)
        {
            StringBuilder sb = new StringBuilder();

            PropertyDataCollection properties = managementBaseObject.Properties;

            sb.AppendLine("ManagementBaseObject Property Names:");
            foreach (PropertyData property in properties)
            {
                sb.AppendLine(property.Name + "\t");
                foreach (QualifierData q in property.Qualifiers)
                {
                    //if (q.Name.Equals("Description"))
                    //{
                    //    sb.AppendLine(
                    //        processClass.GetPropertyQualifierValue(
                    //            property.Name, q.Name).ToString());
                    //}
                }
            }
            string a = sb.ToString();
        }
    }
}
