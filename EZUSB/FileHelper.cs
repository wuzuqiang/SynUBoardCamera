using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EZUSB
{
    public class FileHelper
    {
        public static string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now + "Log.txt");
        static FileHelper()
        {
            System.IO.File.Create(filePath);
        }
        public static void WriteText(string text)
        {   //通过直接复制Debug下文件，执行应用程序的时候，有时会出现没有文件权限而产生异常
            File.AppendAllText(filePath, DateTime.Now + "：\r\n" + text);
        }
    }
}
