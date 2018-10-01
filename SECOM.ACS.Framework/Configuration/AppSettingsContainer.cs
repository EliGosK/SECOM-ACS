using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Caching;

namespace SECOM.ACS.Configuration
{
    public class AppSettingsContainer : IDisposable
    {
        public string ApplicationName { get; set; }
        public string CompanyName { get; set; }
        public string Version { get; set; }

        public string MapImageFolder { get; set; } = "~/content/map";
        public string SuspendFile { get; set; } = "~/app_suspend.json";
        public string ApplicationLogFolder { get; set; } = "~/logging";

        public KendoUIConfiguration KendoUI { get; set; } = new KendoUIConfiguration();
        public AuthenticationConfiguration Authentication { get; set; } = new AuthenticationConfiguration();
        public MailConfiguration Mail { get; set; } = new MailConfiguration();
        public TaskConfiguration Task { get; set; } = new TaskConfiguration();
        public CardConfiguration Card { get; set; } = new CardConfiguration();
        public AcsEmployeeConfiguration AcsEmployee { get; set; } = new AcsEmployeeConfiguration();
        public AcsVisitorConfiguration AcsVisitor { get; set; } = new AcsVisitorConfiguration();
        public HostedProcessConfiguration HostedProcess { get; set; } = new HostedProcessConfiguration();
        public CultureConfiguration Cultures { get; set; } = new CultureConfiguration();

        public void Dispose()
        {
            
        }
    }
}
