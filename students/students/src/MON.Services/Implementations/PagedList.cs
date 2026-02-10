namespace MON.Services
{
    using MON.Shared.Interfaces;
    using System.Collections.Generic;

    public class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            Items = new List<T>();
        }

        public int TotalCount { get; set; }

        public IList<T> Items { get; set; }

        public string ItemsAsJsonStr { get; set; }
    }
}
