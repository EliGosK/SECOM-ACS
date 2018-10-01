namespace CSI.Enumerations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreDisplayAttribute : Attribute
    {
        public IgnoreDisplayAttribute() : this(true)
        {
        }

        public IgnoreDisplayAttribute(bool ignore)
        {
            this.Ignore = ignore;
        }

        public bool Ignore { get; private set; }
    }
}

