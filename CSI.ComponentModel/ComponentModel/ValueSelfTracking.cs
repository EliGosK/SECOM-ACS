using System;
using System.Runtime.CompilerServices;

namespace CSI.ComponentModel
{
    public class ValueSelfTracking<T> where T: struct
    {
        private T currentValue;
        private bool enableUpdateOrigianal;

        public ValueSelfTracking()
        {
            this.currentValue = default(T);
            this.enableUpdateOrigianal = true;
            this.AllowOnceChanged = true;
        }

        public ValueSelfTracking(bool allowOnceChanged)
        {
            this.currentValue = default(T);
            this.enableUpdateOrigianal = true;
            this.AllowOnceChanged = allowOnceChanged;
        }

        public virtual void Reset()
        {
            this.enableUpdateOrigianal = true;
        }

        public bool AllowOnceChanged {get; private set;}

        public virtual bool HasChanged
        {
            get
            {
                return !this.Value.Equals(this.OriginalValue);
            }
        }

        public T OriginalValue {get; private set;}

        public T Value
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                if (!this.currentValue.Equals(value) && this.enableUpdateOrigianal)
                {
                    this.OriginalValue = value;
                    if (this.AllowOnceChanged)
                    {
                        this.enableUpdateOrigianal = false;
                    }
                }
                this.currentValue = value;
            }
        }
    }
}

