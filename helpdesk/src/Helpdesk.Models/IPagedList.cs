using System.Collections.Generic;

namespace Helpdesk.Models
{
    public interface IPagedList<T>
    {
        public int TotalCount { get; set; }
        public IList<T> Items { get; set; }
        public string ItemsAsJsonStr { get; set; }
    }
}
