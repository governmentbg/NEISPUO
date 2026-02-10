
using MON.Shared.Enums;
using MON.Shared.Interfaces;

namespace MON.DataAccess
{
    public partial class RelocationDocument : ICreatable, IUpdatable, IAuditable
    {
        public bool IsDraft => Status == (int)DocumentStatus.Draft;

        public short SchoolYear => this.SendingInstitutionSchoolYear;

    }
}