using System.Collections.Generic;

namespace MON.Models.StudentModels.Search
{
    public class StudentSearchModelExtended : StudentSearchModel, IPagedListInput
    {
        // От SPA-то идват тези възможни стойности за сортиране (в грида има тези колони), 
        // но тези пропъртита съществуват във view модела, а не в ентитито. 
        private static readonly HashSet<string> skipSort = new HashSet<string>(new[] { "age", "district", "municipality", "school", "age asc", "district asc", "municipality asc", "school asc", "age desc", "district desc", "municipality desc", "school desc" });
        private static readonly string defaultSort = "PersonId desc";
        public StudentSearchModelExtended()
        {
            PageIndex = 0;
            PageSize = 10;
            SortBy = defaultSort;
        }
        public string Filter { get; set; }

        private string _sortBy;

        public string SortBy
        {
            get => string.IsNullOrWhiteSpace(_sortBy)
                    ? defaultSort
                    : skipSort.Contains(_sortBy.ToLower())
                        ? defaultSort
                        : _sortBy;
                        //_sortBy.Contains("pin", System.StringComparison.OrdinalIgnoreCase)
                        //    ? _sortBy.Replace("pin", "PersonalId")
                        //    : _sortBy;

            set => _sortBy = value;
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
