using CSI.Resources;
using System;
using System.Runtime.CompilerServices;

namespace CSI.ComponentModel
{  
    public class ExceptionAttribute : Attribute
    {
        public ExceptionAttribute(Type exceptionType)
        {
            this.ExceptionType = exceptionType;
        }

        public ExceptionAttribute(Type exceptionType,string message) 
            : this(exceptionType)
        {
            this.ErrorMessage = message;
        }

        public ExceptionAttribute(Type exceptionType, Type resourceType,string name)
            : this(exceptionType)
        {
            this.ResourceType = resourceType;
            this.ResourceName = name;
        }

        public Type ExceptionType { get; private set; }
        public string ErrorMessage { get; private set; }

        public Type ResourceType { get; private set; }
        public string ResourceName { get; private set; }
    }
}

