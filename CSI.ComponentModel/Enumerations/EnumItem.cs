using System;
using System.Runtime.CompilerServices;

namespace CSI.Enumerations
{
    [Serializable]
    public class EnumItem
    {
        public EnumItem(string name, int value)
            : this(name, name, value, 0)
        {
        }

        public EnumItem(string name, int value, int order)
            : this(name, name, value, order)
        {
        }

        public EnumItem(string name, string displayName, int value)
            : this(name, displayName, value, 0)
        {
        }

        public EnumItem(string name, string displayName, int value, int order)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Value = value;
            this.Order = order;
        }

        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}

