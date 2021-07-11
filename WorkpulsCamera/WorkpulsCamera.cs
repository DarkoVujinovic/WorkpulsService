using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace WorkpulsCamera
{
    class WorkpulsCamera
    {
        static void Main(string[] args)
        {
            bool isAvailable = false;

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
            {
                if (key != null)
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                            {
                                var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                if (endTime <= 0)
                                {
                                    isAvailable = true;
                                    String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    string micUsage = "Web Camera is currently in use. | Time: " + DateTime.Now.ToString() + " | User: " + UserName + " |";
                                    Console.WriteLine(micUsage);
                                }
                            }
                        }
                    }
                }                
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam"))
            {
                if (key != null)
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                            {
                                var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                if (endTime <= 0)
                                {
                                    isAvailable = true;
                                    String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                    string micUsage = "Web Camera is currently in use. | Time: " + DateTime.Now.ToString() + " | User: " + UserName + " |";
                                    Console.WriteLine(micUsage);
                                }
                            }
                        }
                    }
                }                
            }

            // if is not available at all
            if (isAvailable == false)
            {
                Console.WriteLine("false");
            }

            return;
        }
    }
}
