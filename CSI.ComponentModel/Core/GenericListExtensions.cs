using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.Core.Extensions
{
    public static class GenericListExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static T FindOne<T>(this IList<T> items, T findItem) where T : class,IEqualityComparer<T>
        {
            foreach (var item in items)
            {
                if (item.Equals(item, findItem)) 
                {
                    return item;
                }
            }
            return null;
        }
    }
}
