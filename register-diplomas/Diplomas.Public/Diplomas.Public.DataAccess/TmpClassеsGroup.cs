using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class TmpClassеsGroup
    {
        public int ClassId { get; set; }
        public short? SchoolYеar { get; set; }
        public int InstitutionId { get; set; }
        public int ClassIdOrig { get; set; }
        public int ClassGroupId { get; set; }
        public string ClassName { get; set; }
        public int? ParentClassId { get; set; }
        public short? BasicClassId { get; set; }
        public string BasicClassName { get; set; }
        public short? ClassTypeId { get; set; }
        public string ClassTypeName { get; set; }
        public byte? AreaId { get; set; }
        public string AreaName { get; set; }
        public int? ClassSpecialityId { get; set; }
        public string ClassSpecialityName { get; set; }
        public int? FltypeId { get; set; }
        public string FltypeName { get; set; }
        public short? ClassEduFormId { get; set; }
        public string ClassEduFormName { get; set; }
        public int? DepartmentAddressId { get; set; }
        public string DepartmentAddressName { get; set; }
        public int? IsSpecNeeds { get; set; }
    }
}
