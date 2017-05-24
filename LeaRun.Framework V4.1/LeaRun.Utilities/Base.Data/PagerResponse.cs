using System;
using System.Collections.Generic;

namespace LeaRun.Utilities
{
    public sealed class PagerResponse<T>
    {
        public PagerResponse( int actualPageIndex, int pageSize, int totalRowCount, IList<T> entities)
        {
            if (pageSize < 0)
            {
                throw new ArgumentException("pageSize must >= 0");
            }
            if (actualPageIndex < 1)
            {
                throw new ArgumentException("pageIndex must >= 1");
            }
            this.PageSize = pageSize;
            this.ActualPageIndex = actualPageIndex;
            this.TotalRowCount = totalRowCount;
            this.Entities = entities;
        }

        public int ActualPageIndex { get; private set; }


        public int PageSize { get; private set; }


        public int TotalRowCount { get; private set; }


        public IList<T> Entities { get; private set; }

        public int PageCount
        {
            get
            {
                int pageCount = TotalRowCount % PageSize == 0 ? TotalRowCount / PageSize : TotalRowCount / PageSize + 1;

                if (pageCount == 0)
                {
                    if (pageCount != 0)
                    {
                        throw new ArgumentException("rowCount must be zero when pageCount is zero");
                    }
                }
                return pageCount;
            }
        }

        public bool IsFirstPage
        {
            get { return this.ActualPageIndex == 1; }
        }

        public bool IsLastPage
        {
            get
            {
                return
                    this.ActualPageIndex == this.PageCount ||
                    this.PageCount == 0;
            }
        }

    }
}
