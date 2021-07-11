using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace WorkpulsServiceDemo
{
    public class Service
    {
        private readonly Timer timer;

        private static readonly object _lockObject = new object();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessId;
            public Int32 dwThreadId;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            IntPtr lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreatePipe(
            ref IntPtr hReadPipe,
            ref IntPtr hWritePipe,
            ref SECURITY_ATTRIBUTES lpPipeAttributes,
            uint nSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint WTSGetActiveConsoleSessionId();
        [DllImport("Wtsapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(UInt32 SessionId, out IntPtr hToken);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint WaitForSingleObject(IntPtr hProcess, uint dwMilliseconds);

        public Service()
        {
            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += OnElapsedTime;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            lock (_lockObject)
            {
                try
                {
                    string[] lines = new string[] { "WorkpulsServiceDemo windows service is ticking: " + DateTime.Now.ToString() + "" };
                    File.AppendAllLines(@"c:\WorkpulsServiceLogFile.txt", lines);

                    GetAppInFocus();              

                    CheckWebCameraUsage();

                    CheckMicrophoneUsage();

                    CheckIsLogOpenByTheUser();

                }
                catch(Exception ex)
                {
                    string[] error = new string[] { "Application exception: " + ex + "" };
                    File.AppendAllLines(@"C:\log-exception.txt", error);
                }
            }
        }

        public void Start()
        {
            timer.Start();
            string[] lines = new string[] { "WorkpulsServiceDemo windows service STARTED at: " + DateTime.Now.ToString() + "" };
            File.AppendAllLines(@"c:\WorkpulsServiceLogFile.txt", lines);
        }
        public void Stop()
        {
            timer.Stop();
            string[] lines = new string[] { "WorkpulsServiceDemo windows service STOPPED at: " + DateTime.Now.ToString() + "" };
            File.AppendAllLines(@"c:\WorkpulsServiceLogFile.txt", lines);
        }

        private static void GetAppInFocus()
        {
            try
            {
                IntPtr read = new IntPtr();
                IntPtr write = new IntPtr();
                IntPtr read2 = new IntPtr();
                IntPtr write2 = new IntPtr();
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();
                saAttr.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                saAttr.bInheritHandle = 1;
                saAttr.lpSecurityDescriptor = IntPtr.Zero;

                CreatePipe(ref read, ref write, ref saAttr, 0);
                CreatePipe(ref read2, ref write2, ref saAttr, 0);

                uint CREATE_NO_WINDOW = 0x08000000;
                int STARTF_USESTDHANDLES = 0x00000100;
                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.hStdOutput = write;
                si.hStdError = write;
                si.hStdInput = read2;
                si.lpDesktop = "Winsta0\\default";
                si.dwFlags = STARTF_USESTDHANDLES;
                PROCESS_INFORMATION pi;

                IntPtr hToken;
                bool err = WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out hToken);
                //string path = @"C:\Users\Darko\source\WinService\WorkpulsConsoleDemo\bin\Debug\WorkpulsConsoleDemo.exe";
                string path = @"C:\Windows\WorkpulsActiveWindow.exe";
                if (CreateProcessAsUser(hToken, path, null, IntPtr.Zero, IntPtr.Zero, true, CREATE_NO_WINDOW, IntPtr.Zero, IntPtr.Zero, ref si, out pi))
                {
                    uint ret = WaitForSingleObject(pi.hProcess, 2000); //wait for the child process exit.
                    if (ret == 0)
                    {
                        byte[] title = new byte[200];
                        uint reads = 0;
                        CloseHandle(write);
                        err = ReadFile(read, title, 200, out reads, IntPtr.Zero);
                        string result = System.Text.Encoding.UTF8.GetString(title).Replace("\0", "").Replace("\r", "").Replace("\n", "");

                        string[] focusApp = new string[] { "Application currently in focus: " + result + "" };
                        File.AppendAllLines(@"C:\log-app.txt", focusApp);

                    }
                }

                CloseHandle(read2);
                CloseHandle(write2);
                CloseHandle(read);
            }
            catch (Exception ex)
            {
                string[] error = new string[] { "Application exception: " + ex + "" };
                File.AppendAllLines(@"C:\log-exception.txt", error);
            }
        }
        private static void CheckWebCameraUsage()
        {
            try
            {
                IntPtr read = new IntPtr();
                IntPtr write = new IntPtr();
                IntPtr read2 = new IntPtr();
                IntPtr write2 = new IntPtr();
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();
                saAttr.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                saAttr.bInheritHandle = 1;
                saAttr.lpSecurityDescriptor = IntPtr.Zero;

                CreatePipe(ref read, ref write, ref saAttr, 0);
                CreatePipe(ref read2, ref write2, ref saAttr, 0);

                uint CREATE_NO_WINDOW = 0x08000000;
                int STARTF_USESTDHANDLES = 0x00000100;
                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.hStdOutput = write;
                si.hStdError = write;
                si.hStdInput = read2;
                si.lpDesktop = "Winsta0\\default";
                si.dwFlags = STARTF_USESTDHANDLES;
                PROCESS_INFORMATION pi;

                IntPtr hToken;
                bool err = WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out hToken);
                string path = @"C:\Windows\WorkpulsCamera.exe";
                if (CreateProcessAsUser(hToken, path, null, IntPtr.Zero, IntPtr.Zero, true, CREATE_NO_WINDOW, IntPtr.Zero, IntPtr.Zero, ref si, out pi))
                {
                    uint ret = WaitForSingleObject(pi.hProcess, 2000); //wait for the child process exit.
                    if (ret == 0)
                    {
                        byte[] title = new byte[200];
                        uint reads = 0;
                        CloseHandle(write);
                        err = ReadFile(read, title, 200, out reads, IntPtr.Zero);
                        string result = System.Text.Encoding.UTF8.GetString(title).Replace("\0", "").Replace("\r", "").Replace("\n", "");

                        if (result != "false")
                        {
                            string[] micUsage = new string[] { result };
                            File.AppendAllLines(@"C:\log-camera.txt", micUsage);
                        }
                    }
                }

                CloseHandle(read2);
                CloseHandle(write2);
                CloseHandle(read);
            }
            catch (Exception ex)
            {
                string[] error = new string[] { "Application exception: " + ex + "" };
                File.AppendAllLines(@"C:\log-exception.txt", error);
            }

        }
         private static void CheckMicrophoneUsage()
        {
            try
            {
                IntPtr read = new IntPtr();
                IntPtr write = new IntPtr();
                IntPtr read2 = new IntPtr();
                IntPtr write2 = new IntPtr();
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();
                saAttr.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                saAttr.bInheritHandle = 1;
                saAttr.lpSecurityDescriptor = IntPtr.Zero;

                CreatePipe(ref read, ref write, ref saAttr, 0);
                CreatePipe(ref read2, ref write2, ref saAttr, 0);

                uint CREATE_NO_WINDOW = 0x08000000;
                int STARTF_USESTDHANDLES = 0x00000100;
                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.hStdOutput = write;
                si.hStdError = write;
                si.hStdInput = read2;
                si.lpDesktop = "Winsta0\\default";
                si.dwFlags = STARTF_USESTDHANDLES;
                PROCESS_INFORMATION pi;

                IntPtr hToken;
                bool err = WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out hToken);
                string path = @"C:\Windows\WorkpulsMicrophone.exe";
                if (CreateProcessAsUser(hToken, path, null, IntPtr.Zero, IntPtr.Zero, true, CREATE_NO_WINDOW, IntPtr.Zero, IntPtr.Zero, ref si, out pi))
                {
                    uint ret = WaitForSingleObject(pi.hProcess, 2000); //wait for the child process exit.
                    if (ret == 0)
                    {
                        byte[] title = new byte[200];
                        uint reads = 0;
                        CloseHandle(write);
                        err = ReadFile(read, title, 200, out reads, IntPtr.Zero);
                        string result = System.Text.Encoding.UTF8.GetString(title).Replace("\0", "").Replace("\r", "").Replace("\n", "");

                        if (result != "false")
                        {
                            string[] micUsage = new string[] { result };
                            File.AppendAllLines(@"C:\log-camera.txt", micUsage);
                        }
                    }
                }

                CloseHandle(read2);
                CloseHandle(write2);
                CloseHandle(read);
            }
            catch (Exception ex)
            {
                string[] error = new string[] { "Application exception: " + ex + "" };
                File.AppendAllLines(@"C:\log-exception.txt", error);
            }
        }

        private static void CheckIsLogOpenByTheUser()
        {
            try
            {
                IntPtr read = new IntPtr();
                IntPtr write = new IntPtr();
                IntPtr read2 = new IntPtr();
                IntPtr write2 = new IntPtr();
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();
                saAttr.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                saAttr.bInheritHandle = 1;
                saAttr.lpSecurityDescriptor = IntPtr.Zero;

                CreatePipe(ref read, ref write, ref saAttr, 0);
                CreatePipe(ref read2, ref write2, ref saAttr, 0);

                uint CREATE_NO_WINDOW = 0x08000000;
                int STARTF_USESTDHANDLES = 0x00000100;
                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.hStdOutput = write;
                si.hStdError = write;
                si.hStdInput = read2;
                si.lpDesktop = "Winsta0\\default";
                si.dwFlags = STARTF_USESTDHANDLES;
                PROCESS_INFORMATION pi;

                IntPtr hToken;
                bool err = WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out hToken);
                string path = @"C:\Windows\WorkpulsLogCheck.exe";
                if (CreateProcessAsUser(hToken, path, null, IntPtr.Zero, IntPtr.Zero, true, CREATE_NO_WINDOW, IntPtr.Zero, IntPtr.Zero, ref si, out pi))
                {
                    uint ret = WaitForSingleObject(pi.hProcess, 2000); //wait for the child process exit.
                    if (ret == 0)
                    {
                        byte[] title = new byte[200];
                        uint reads = 0;
                        CloseHandle(write);
                        err = ReadFile(read, title, 200, out reads, IntPtr.Zero);
                        string result = System.Text.Encoding.UTF8.GetString(title).Replace("\0", "").Replace("\r", "").Replace("\n", "");

                        if (result != "false")
                        {
                            string[] logOpen = new string[] { result };
                            File.AppendAllLines(@"C:\log-open-log-file.txt", logOpen);
                        }
                    }
                }

                CloseHandle(read2);
                CloseHandle(write2);
                CloseHandle(read);
            }
            catch (Exception ex)
            {
                string[] error = new string[] { "Application exception: " + ex + "" };
                File.AppendAllLines(@"C:\log-exception.txt", error);
            }
        }
    }
}
