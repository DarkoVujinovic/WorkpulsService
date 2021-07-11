using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace WorkpulsServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(service => new Service());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("WorkpulsServiceDemo");
                x.SetDisplayName("WorkpulsServiceDemo");
                x.SetDescription("WorkpulsServiceDemo user actions monitoring application service.");

            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
