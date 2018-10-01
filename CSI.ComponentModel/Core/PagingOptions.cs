using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.Core
{
    public interface ISupportPagingOptions
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        int EndItemIndex { get; }
    }

    public class PagingOptions : ISupportPagingOptions
    {
        public PagingOptions()
        {
            this.PageIndex = 0;
            this.PageSize = Int32.MaxValue;
        }

        /// <summary>
        /// Set or get zero base page index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Set or get number of records display in page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Get item start index
        /// </summary>
        public int StartItemIndex
        {
            get
            {

                return (Math.Max(0, this.PageIndex - 1) * PageSize) + (PageIndex == 1 ? 0 : 1);
            }
        }

        /// <summary>
        /// Get item end index
        /// </summary>
        public int EndItemIndex
        {
            get
            {
                return this.StartItemIndex + PageSize - (PageIndex == 1 ? 0 : 1);
            }
        }
    }
}
