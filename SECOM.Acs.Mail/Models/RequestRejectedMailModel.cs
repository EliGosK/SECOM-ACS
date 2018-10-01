﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail.Models
{
    [MailTemplate("RequestRejected")]
    public class RequestRejectedMailModel : RequestMailModeBase
    {
        public string RejectReason { get; set; }
    }
}
