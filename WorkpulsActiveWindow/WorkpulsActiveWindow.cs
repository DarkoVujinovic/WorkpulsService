using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace WorkpulsActiveWindow
{
    class WorkpulsActiveWindow
    {
        [DllImport("User32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        static void Main(string[] args)
        {
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

            String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string concat = title + " | Date and Time: "+ DateTime.Now.ToString() +" | Used by the user: " + UserName;
            Console.WriteLine(concat);
            return;
        }
    }
}
