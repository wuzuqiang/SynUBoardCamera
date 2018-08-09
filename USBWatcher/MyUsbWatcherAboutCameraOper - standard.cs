using System;
using System.Management;
using System.Text;

namespace Software.Util
{
    /// <summary>
    /// 监视USB摄像头插拔
    /// </summary>
    public class UsbCameraWatcher
    {
        /// <summary>
        /// USB插入事件监视
        /// </summary>
        private ManagementEventWatcher insertWatcher = null;
        /// <summary>
        /// USB拔出事件监视
        /// </summary>
        private ManagementEventWatcher removeWatcher = null;

        /// <summary>
        /// 添加USB事件监视器
        /// </summary>
        /// <param name="usbInsertHandler">USB插入事件处理器</param>
        /// <param name="usbRemoveHandler">USB拔出事件处理器</param>
        /// <param name="withinInterval">发送通知允许的滞后时间</param>
        private Boolean AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler, EventArrivedEventHandler usbRemoveHandler, TimeSpan withinInterval)
        {
            try
            {
                ManagementScope Scope = new ManagementScope("root\\CIMV2");
                Scope.Options.EnablePrivileges = true;

                // USB插入监视
                if (usbInsertHandler != null)
                {
                    WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_PnPEntity'");

                    insertWatcher = new ManagementEventWatcher(Scope, InsertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                // USB拔出监视
                if (usbRemoveHandler != null)
                {
                    WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_PnPEntity'");

                    removeWatcher = new ManagementEventWatcher(Scope, RemoveQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();
                }

                return true;
            }

            catch
            {
                RemoveUSBEventWatcher();
                return false;
            }
        }

        /// <summary>
        /// 移去USB事件监视器
        /// </summary>
        private void RemoveUSBEventWatcher()
        {
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }

            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }

        /// <summary>
        /// 定位发生插拔的PnPEntity设备
        /// </summary>
        /// <param name="e">插拔事件参数</param>
        /// <returns>发生插拔现象的Camera设备</returns>
        private ExtPnPEntityInfo[] WhoUSBControllerDevice(EventArrivedEventArgs e)
        {
            ManagementBaseObject Entity = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (Entity != null && Entity.ClassPath.ClassName == "Win32_PnPEntity")
            {
                string pnpClass = Entity["PnPClass"] as String;
                if (pnpClass != "Camera" && pnpClass != "Image")
                    return null;
                Guid theClassGuid = new Guid(Entity["ClassGuid"] as String);    // 设备安装类GUID
                ExtPnPEntityInfo Element = new ExtPnPEntityInfo();
                Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // 设备ID
                Element.Name = Entity["Name"] as String;                // 设备名称
                Element.Description = Entity["Description"] as String;  // 设备描述
                Element.CompatibleID = Entity["CompatibleID"] as String[];
                Element.PnPClass = pnpClass;
                return new ExtPnPEntityInfo[1] { Element };
            }
            return null;
        }

        /// <summary>
        /// 添加监视器
        /// </summary>
        public Boolean AddWatcher()
        {
            return AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// 摄像头插入事件
        /// </summary>
        public Action<String> EventCameraInsert;
        /// <summary>
        /// 摄像头移除事件
        /// </summary>
        public Action<String> EventCameraRemove;

        private void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            ExtPnPEntityInfo[] pnPEntityInfos = WhoUSBControllerDevice(e);  //usb拔插后判断插入的是否是摄像头
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                //USB插入
                if (pnPEntityInfos != null)
                {   //表示得到了摄像头
                    EventCameraInsert(pnPEntityInfos[0].Name);
                }
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {   //USB拔出
                //如果是摄像头的拔出
                if (pnPEntityInfos != null)
                {   //表示得到了摄像头
                    EventCameraRemove(pnPEntityInfos[0].Name);
                }
            }
        }

        public void DelWatcher()
        {
            RemoveUSBEventWatcher();
        }

        ~UsbCameraWatcher()
        {
            DelWatcher();
        }

        /// <summary>
        /// 扩展的即插即用设备信息结构
        /// </summary>
        public struct ExtPnPEntityInfo
        {
            public String PNPDeviceID;      // 设备ID
            public String Name;             // 设备名称
            public String Description;      // 设备描述
            public String Service;          // 服务
            public String Status;           // 设备状态
            public UInt16 VendorID;         // 供应商标识
            public UInt16 ProductID;        // 产品编号 
            public Guid ClassGuid;          // 设备安装类GUID
            public String PnPClass;         //PNPClass 属性包含此即插即用设备的类型名称
            public String[] CompatibleID;   //设备的兼容 ID 列表
        }
    }
}
