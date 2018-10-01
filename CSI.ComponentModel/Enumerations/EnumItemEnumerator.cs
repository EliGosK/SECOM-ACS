namespace CSI.Enumerations
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class EnumItemEnumerator : IEnumerator
    {
        private FieldInfo[] fieldInfo;
        private int index;

        public EnumItemEnumerator(FieldInfo[] fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            this.index = 0;
        }

        public bool MoveNext()
        {
            if (this.index < (this.fieldInfo.Length - 1))
            {
                this.index++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.index = 0;
        }

        public object Current
        {
            get
            {
                return new EnumItem(this.fieldInfo[this.index].Name, (int) this.fieldInfo[this.index].GetValue(null));
            }
        }
    }
}

