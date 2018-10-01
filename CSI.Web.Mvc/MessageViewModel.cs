using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSI.Web.Mvc
{
    public class MessageViewModel
    {
        public string Title { get; set; } = String.Empty;
        public string Message { get; set; } = String.Empty;
        public IList<string> MessageList { get; set; } = new List<string>();
        public ApplicationMessageType MessageType { get; set; } = ApplicationMessageType.Info;

        public static MessageViewModel Success(string message)
        {
            return new MessageViewModel() { Title = "Success", MessageType = ApplicationMessageType.Success, Message = message };
        }

        public static MessageViewModel Success(string title, string message)
        {
            return new MessageViewModel() { Title = title, MessageType = ApplicationMessageType.Success, Message = message };
        }

        public static MessageViewModel Error(string message)
        {
            return new MessageViewModel() { Title = "Application Error", MessageType = ApplicationMessageType.Error, Message = message };
        }

        public static MessageViewModel Error(string title, string message)
        {
            return new MessageViewModel() { Title = title, MessageType = ApplicationMessageType.Error, Message = message };
        }

        public static MessageViewModel Info(string message)
        {
            return new MessageViewModel() { Title = "Information", MessageType = ApplicationMessageType.Info, Message = message };
        }

        public static MessageViewModel Info(string title,string message)
        {
            return new MessageViewModel() { Title = title, MessageType = ApplicationMessageType.Info, Message = message };
        }

        public static MessageViewModel Warning(string message)
        {
            return new MessageViewModel() { Title = "Warning", MessageType = ApplicationMessageType.Warning, Message = message };
        }

        public static MessageViewModel Warning(string title,string message)
        {
            return new MessageViewModel() { Title = title, MessageType = ApplicationMessageType.Warning, Message = message };
        }
    }
    
    public enum ApplicationMessageType
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Success = 8
    }
}