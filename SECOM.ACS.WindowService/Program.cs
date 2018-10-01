using Atlas;
using Autofac;
using Common.Logging;
using CSI.IO;
using System;
using System.Configuration;
using System.Linq;

namespace SECOM.ACS.WindowsService
{
    /// <summary>
    /// This represents the Windows Service application entity.
    /// </summary>
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("logger");

        /// <summary>
        /// This represents the main entry point of the Windows Service application.
        /// </summary>
        /// <param name="args">List of arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                var version = System.Reflection.Assembly.GetAssembly(typeof(Program)).GetName().Version;
                Log.Info($"Initialize Access Control Service (Version: {version})");
                var configuration = Host.UseAppConfig<AccessControlHostedProcess>()  
                                        .AllowMultipleInstances()    
                                        .WithRegistrations(p => p.RegisterModule(new AccessControlModule()));  
                if (args != null && args.Any())
                    configuration = configuration.WithArguments(args);  

                Host.Start(configuration);                              
            }
            catch (Exception ex)
            {
                Log.Fatal("Exception during startup.", ex);
                Console.ReadLine();
            }
        }
    }
}