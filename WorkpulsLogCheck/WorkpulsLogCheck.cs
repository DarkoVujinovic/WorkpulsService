using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Management;

namespace WorkpulsLogCheck
{
    class WorkpulsLogCheck
    {
        [DllImport("User32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static readonly string logFilePath = @"c:\log-app.txt";
        static readonly string logFileName = "log-app.txt";
        static readonly string logFileNoExtension = "log-app";

        static void Main(string[] args)
        {
            bool isInUse = false;
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            string title = "---";
            IntPtr handle;

            //Run GetForeGroundWindows and get active window informations
            handle = GetForegroundWindow();
            if (GetWindowText(handle, buff, nChars) > 0)
            {
                title = buff.ToString();
            }
            
            //check both file path and file name in the title
            if (title.ToLower().Contains(logFilePath) || title.ToLower().Contains(logFileName) || title.ToLower().Contains(logFileNoExtension)) 
            {
                isInUse = true;
                String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string concat = "File '"+ logFilePath + "' --> has been opened by the User: " + UserName + " | Openning time: " + DateTime.Now.ToString() + " ";
                Console.WriteLine(concat);
            }

            // if it is not in use at all
            if (isInUse == false)
            {
                Console.WriteLine("false");
            }

            return;
        }
    }
}
