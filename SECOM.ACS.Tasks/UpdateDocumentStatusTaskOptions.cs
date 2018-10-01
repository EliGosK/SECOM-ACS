using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class UpdateDocumentStatusTaskOptions : UpdateDocumentExpirationOptions
    {
        
    }

    public class UpdateDocumentExpirationOptions
    {
        /// <summary>
        /// Default: Document set expiry in 30 Day
        /// </summary>
        public UpdateDocumentTypes DocumentTypes { get; set; } = UpdateDocumentTypes.All;
        /// <summary>
        /// Set or get enabled notification to requester
        /// </summary>
        public bool EnabledNotification { get; set; } = false;
        public string User { get; set; } = "ACP020";
    }

    [Flags]
    public enum UpdateDocumentTypes : int
    {
        AcsEmployee = 1,
        AcsVisitor = 2,
        AcsItemOut = 4,
        AcsItemIn = 8,
        AcsPhoto = 16,
        All = AcsEmployee | AcsVisitor | AcsItemIn | AcsItemOut  | AcsPhoto
    }
}
