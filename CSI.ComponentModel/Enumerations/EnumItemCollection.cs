namespace CSI.Enumerations
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class EnumItemCollection : IList, ICollection, IEnumerable
    {
        public EnumItemCollection(Type enumType)
        {
            if (enumType.IsEnum)
            {
                this.EnumType = enumType;
            }
        }

        public int Add(object value)
        {
            return 0;
        }

        public void Clear()
        {
        }

        public bool Contains(object value)
        {
            return false;
        }

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator GetEnumerator()
        {
            return new EnumItemEnumerator(this.EnumType.GetFields());
        }

        public int IndexOf(object value)
        {
            return 0;
        }

        public void Insert(int index, object value)
        {
        }

        public void Remove(object value)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public int ValueFromName(string name)
        {
            foreach (FieldInfo info in this.EnumType.GetFields())
            {
                if (name == info.Name)
                {
                    return (int) info.GetValue(null);
                }
            }
            return -1;
        }

        public int Count
        {
            get
            {
                return (this.EnumType.GetFields().Length - 1);
            }
        }

        public Type EnumType { get; private set; }

        public bool IsFixedSize
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object this[int index]
        {
            get
            {
                FieldInfo[] fields = this.EnumType.GetFields();
                return new EnumItem(fields[index + 1].Name, (int) fields[index + 1].GetValue(null));
            }
            set
            {
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }
    }
}

