using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CSI.ComponentModel
{
    public class GenericObjectComparer<T> : IComparer<T> where T: class
    {
        public GenericObjectComparer(string expression)
        {
            this.Expression = expression;
        }

        public int Compare(T x, T y)
        {
            return ObjectComparerHelper.Compare<T>(x, y, this.Expression);
        }

        public string Expression {get ; private set;}
       
    }
}

