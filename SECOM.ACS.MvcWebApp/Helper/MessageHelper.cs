using CSI.Exceptions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public class MessageHelper
    {
        /// <summary>
        ///  MSG0002
        /// </summary>
        /// <returns></returns>
        public static string SystemSuspend()
        {
            return MessageResource.MSG0001;
        }

        /// <summary>
        ///  MSG0002
        /// </summary>
        /// <returns></returns>
        public static string AccessDenied()
        {
            return MessageResource.MSG0002;
        }

        /// <summary>
        /// MSG0003
        /// </summary>
        /// <returns></returns>
        public static string DataNotFound(string detail = "")
        {
            if (String.IsNullOrEmpty(detail))
            {
                return MessageResource.MSG0003;
            }
            return String.Concat(MessageResource.MSG0003," ", detail);
        }

        public static string DataNotFoundFormat(string format,params object[] args)
        {
            return DataNotFound(String.Format(format, args));
        }

        /// <summary>
        /// MSG0004
        /// </summary>
        /// <returns></returns>
        public static string ConfirmDelete()
        {
            return MessageResource.MSG0004;
        }

        /// <summary>
        /// MSG0005
        /// </summary>
        /// <returns></returns>
        public static string DeleteCompleted()
        {
            return MessageResource.MSG0005;
        }

        /// <summary>
        /// MSG0006
        /// </summary>
        /// <returns></returns>
        public static string DeleteFailed(string detail = "")
        {
            return string.Format(MessageResource.MSG0006,detail);
        }

        /// <summary>
        /// MSG0007
        /// </summary>
        /// <returns></returns>
        public static string ConfirmSave()
        {
            return MessageResource.MSG0007;
        }

        /// <summary>
        /// MSG0008
        /// </summary>
        /// <returns></returns>
        public static string SaveCompleted(string detail = "")
        {
            if (String.IsNullOrEmpty(detail))
                return MessageResource.MSG0008;
            return String.Concat(MessageResource.MSG0008, " ", detail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static string SaveFailed(string detail = "")
        {
            return string.IsNullOrEmpty(detail) ? "Save error" : String.Concat(MessageResource.MSG0006, " Error Message: ", detail);
        }

        /// <summary>
        /// MSG0009
        /// </summary>
        /// <returns></returns>
        public static string ConfirmCancel()
        {
            return MessageResource.MSG0009;
        }

        /// <summary>
        /// MSG0010
        /// </summary>
        /// <returns></returns>
        public static string RequiredField(string name)
        {
            return String.Format(MessageResource.MSG0010, name);
        }

        /// <summary>
        /// MSG0011
        /// </summary>
        /// <returns></returns>
        public static string DuplicateField(params object[] args)
        {
            return String.Format(MessageResource.MSG0011,args);
        }

        /// <summary>
        /// MSG0013
        /// </summary>
        /// <returns></returns>
        public static string RequiredDropDownList(string name)
        {
            return String.Format(MessageResource.MSG0013, name);
        }

        /// <summary>
        /// MSG0014
        /// </summary>
        /// <returns></returns>
        public static string ConfirmReset()
        {
            return MessageResource.MSG0014;
        }

        /// <summary>
        /// MSG0015
        /// </summary>
        /// <returns></returns>
        public static string DuplicateData(params object[] args)
        {
            return String.Format(MessageResource.MSG0015, args);
        }

        /// <summary>
        /// MSG0016
        /// </summary>
        /// <returns></returns>
        public static string DeleteFailedDataInUsed(string name)
        {
            return String.Format(MessageResource.MSG0016, name);
        }

        /// <summary>
        /// MSG0019
        /// </summary>
        /// <returns></returns>
        public static string SuperiorApprovalDataNotFound()
        {
            return MessageResource.MSG0019;
        }

        /// <summary>
        /// MSG0020
        /// </summary>
        /// <returns></returns>
        public static string ConfirmReject()
        {
            return MessageResource.MSG0020;
        }

        /// <summary>
        /// MSG0021
        /// </summary>
        /// <returns></returns>
        public static string ConfirmApprove()
        {
            return MessageResource.MSG0021;
        }

        /// <summary>
        /// MSG0021
        /// </summary>
        /// <returns></returns>
        public static string ConfirmForceDone()
        {
            return MessageResource.MSG0022;
        }

        /// <summary>
        /// MSG0023
        /// </summary>
        /// <returns></returns>
        public static string ConfirmAcknowledge()
        {
            return MessageResource.MSG0023;
        }

        /// <summary>
        /// MSG0032
        /// </summary>
        /// <returns></returns>
        public static string DuplicateList(string CardNo, string VisitorName)
        {
            return String.Format(MessageResource.MSG0032, CardNo, VisitorName);
        }

        /// <summary>
        /// MSG0031
        /// </summary>
        /// <returns></returns>
        public static string DuplicateInList(string name)
        {
            return String.Format(MessageResource.MSG0031, name);
        }

        /// <summary>
        /// MSG0021
        /// </summary>
        /// <returns></returns>
        public static string ConfirmCancelRequest()
        {
            return MessageResource.ConfirmCancelRequest;
        }

        public static string InvalidModelState(ModelStateDictionary state = null)
        {
            if (state == null)
                return String.Format("Invalid request");
            else {
                var sb = new StringBuilder("Invalid request");
                foreach (var value in state.Values)
                {
                    if (value.Errors != null && value.Errors.Count > 0) {
                        foreach (var error in value.Errors)
                        {
                            sb.Append(error.ErrorMessage);
                        }
                    }
                }
                return sb.ToString();
            }

        }

        public static string RequestAlreadyCompleted()
        {
            return String.Format("Invalid request data. Request is already complete");
        }

        public static string InvalidReqApprovalRequest()
        {
            return MessageResource.InvalidReqApprovalRequest;
        }

        public static string InvalidRequestForCancelRequest()
        {
            return MessageResource.InvalidRequestStatusForCancelRequest;
        }

        public static string InvalidRequestForForceDone()
        {
            return "Invalid request status to force done.";
        }

        public static string InvalidRequestStatusForGA()
        {
            return "Invalid request status to acknowledge from GA.";
        }

        public static string InvalidRequestForGAReturnAsset()
        {
            return "Invalid request status to update return asset";
        }

        public static string ConfirmClearEmployeeList()
        {
            return MessageResource.ConfirmClearEmployeeList;
        }

        public static string ConfirmClearVisitorList()
        {
            return MessageResource.ConfirmClearVisitorList;
        }

        public static string GenerateReportSuccess(string name)
        {
            return $"Generate {name} report is successfully.";
        }

        public static string InvalidInputAreaApprover(string name)
        {
            return $"Invalid input area approver ({name})";
        }

        public static string WorkflowError(Exception error)
        {
            return String.Format("Internal error in workflow system. Error {0}", ExceptionUtility.GetLastExceptionMessage(error));
        }

        public static string ConfirmRunTask()
        {
            return MessageResource.ConfirmRunTask;
        }

        public static string ConfirmReceiveCard()
        {
            return MessageResource.ConfirmReceiveCard;
        }

        public static string ConfirmReturnCard()
        {
            return MessageResource.ConfirmReturnCard;
        }

        public static string PasswordResetCompleted()
        {
            return MessageResource.PasswordResetCompleted;
        }
    }
}