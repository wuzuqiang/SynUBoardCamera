using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Software.Util;

namespace USBWatcher
{
    public partial class FrmUseUsbCameraWatcher : Form
    {
        public FrmUseUsbCameraWatcher()
        {
            InitializeComponent();
        }

        UsbCameraWatcher usbCameraWatcher = new UsbCameraWatcher();
        private void FrmUseUsbCameraWatcher_Load(object sender, EventArgs e)
        {
            //string strResult = ;
            if(usbCameraWatcher.AddWatcher())
            {
                usbCameraWatcher.EventCameraInsert += UsbCameraWatcher_EventCameraInsert;
                usbCameraWatcher.EventCameraRemove += UsbCameraWatcher_EventCameraRemove;
            }
        }

        private void UsbCameraWatcher_EventCameraRemove(string obj)
        {
            SetText("移除设备：" + obj);
        }

        private void UsbCameraWatcher_EventCameraInsert(string obj)
        {
            SetText("插入设备：" + obj);
        }


        // 对 Windows 窗体控件进行线程安全调用
        private void SetText(String text)
        {
            text = text + "\n";
            if (this.textBox1.InvokeRequired)
            {
                this.textBox1.BeginInvoke(new Action<String>((msg) =>
                {
                    this.textBox1.AppendText(msg);
                }), text);
            }
            else
            {
                this.textBox1.AppendText(text);
            }
        }

    }
}
