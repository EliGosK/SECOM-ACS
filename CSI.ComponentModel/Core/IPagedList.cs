using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.Core
{
    public interface IPagedList<T> : IList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalRecords { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
