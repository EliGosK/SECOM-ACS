using System;
using System.Runtime.CompilerServices;

namespace CSI.ComponentModel
{
    public class AttributeNotFoundException : ApplicationException
    {
        public AttributeNotFoundException(Type attrType) : base("Attirbute not found or place on target")
        {
        }

        public AttributeNotFoundException(Type attrType, string message) : base(message)
        {
            this.TargetAttributeType = attrType;
        }

        public Type TargetAttributeType { get; private set; }
    }
}

