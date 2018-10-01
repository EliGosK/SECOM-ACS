using CSI.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public class RequestStatus
    {
        public const string Requesting = "01";
        public const string Approving = "02";
        public const string Approved = "03";
        public const string Done = "04";
        public const string Cancel = "05";
        public const string Rejected = "06";
        public const string Expired = "07";

        public static bool IsCompleted(string status)
        {
            switch (status)
            {
                case Approved:
                case Done:
                case Cancel:
                case Rejected:
                case Expired:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum TransactionStatus : byte
    {
        CancelTransaction = 0,
        ActiveWithoutAssignedCardID = 1,
        SendCardToACS = 2,
        CancelSendCardToACS = 3,
        SendCardToCancel = 4
    }

    public class CardType
    {
        public const string Employee = "EMN";
        public const string BusinessTrip = "EMT";
        public const string VIP = "VIP";
        public const string Visitor = "VST";
    }

    public class CardRecieveType
    {
        public const string Receive = "01";
        public const string Return = "02";
    }

    public class CardStatus
    {
        public const string Active = "1";
        public const string Inactive = "0";
    }

    public class CardVIPStatus
    {
        public const string Available = "2";
        public const string Unavailable = "1";
    }

    public class ItemStatus
    {
        public const string Active = "1";
        public const string Inactive = "0";
    }

    public class PhotoByTypes
    {
        public const string Employee = "E";
        public const string OutSource = "O";
    }

    public class ApprovalCode
    {
        public const string Approve = "AP";
        public const string Reject = "RJ";
    }

    public class MiscTypes
    {
        public const string VerifyType = "VRF";
        public const string ItemType = "ITM";
        public const string VIPPosition = "PST";
    }

    public class SystemMiscTypes
    {
        public const string Factory = "Factory";
        public const string Status = "Status";
        public const string RequestFor = "ReqFor";
        public const string RequestType = "RequestType";
        public const string ApprovalCode = "ApprovalCode";
        public const string DocumentType = "DocType";
        public const string RegisterCardType = "RegisterCardType";
        public const string Dashboard = "Dashboard";
        public const string PositionLevel = "PositionLevel";
        public const string SpecialPosition = "SpecialPosition";
    }

    public class AcsRequestTypes
    {
        public const string ItemOut = "1";
        public const string ItemIn = "2";
        public const string Photographing = "3";
    }

    public class AcsRequestTypeCodes
    {
        public const string Employee = "ACS020";
        public const string Visitor = "ACS030";
        public const string ItemIn = "ACS050";
        public const string ItemOut = "ACS040";
        public const string Photographing = "ACS060";
        public const string VIP = "ACS090";
    }

    public class AcsRequestPrefixCharacters
    {
        public const string Photographing = "P";
        public const string ItemIn = "I";
        public const string ItemOut = "T";
        public const string Employee = "E";
        public const string Visitor = "V";
        public const string VIP = "S";
    }

    public class PermissionNames
    {
        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Create = "Create";
        public const string Delete = "Delete";
        public const string NewRevision = "Revision";
        public const string Export = "Export";
        public const string ResetPassword = "ResetPassword";
    }

    public class RequestFors
    {
        public const string Employee = "1";
        public const string BusinessTrip = "2";
    }

    public class MiscStatus
    {
        public const string Active = "1";
        public const string Inactive = "0";
    }


    public class AreaStatus
    {
        public const string Active = "1";
        public const string Inactive = "0";
    }

    public class VisitorVerifyTypes
    {
        public const int IDCard = 5;
        public const int PassportNo = 6;
    }

    public class VIPCardStatus
    {
        public const string Available = "2";
        public const string Unavailable = "1";
    }

    public class ReceiveCardTypes
    {
        public const string Received = "01";
        public const string Returned = "02";
    }

    public class PurposeRequestTypes
    {
        public const string Employee = "PRE";
        public const string Visitor = "PRV";
        public const string ItemIn = "PRI";
        public const string ItemOut = "PRT";
        public const string Photo = "PRP";
    }

}
