namespace MON.Shared.Interfaces
{
    using System.Collections.Generic;

    public interface IPagedList<T>
    {
        public int TotalCount { get; set; }
        public IList<T> Items { get; set; }
        public string ItemsAsJsonStr { get; set; }
    }
}
