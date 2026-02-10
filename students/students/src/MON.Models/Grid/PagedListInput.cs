namespace MON.Models
{
    using System;

    public class PagedListInput : IPagedListInput
    {
        private string _sortBy;
        private string _filter;
        public PagedListInput()
        {
            PageIndex = 0;
            PageSize = 10;
        }

        public string SortBy
        {
            get
            {
                return Replace(_sortBy);
            }
            set { _sortBy = value; }
        }

        public string Filter
        {
            get
            {
                return Replace(_filter);
            }
            set { _filter = value; }
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        // https://github.com/advisories/GHSA-4cv2-4hjh-77rx
        private string Replace(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            return value.Replace("assembly", "", StringComparison.OrdinalIgnoreCase)?
                .Replace("config", "", StringComparison.OrdinalIgnoreCase)?
                .Replace("settings", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
