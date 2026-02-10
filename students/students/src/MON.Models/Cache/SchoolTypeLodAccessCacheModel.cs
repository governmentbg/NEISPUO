namespace MON.Models.Cache
{
    using System;

    [Serializable]
    public class SchoolTypeLodAccessCacheModel
    {
        public int DetailedSchoolTypeId { get; set; }
        public bool IsLodAccessAllowed { get; set; }
    }
}
