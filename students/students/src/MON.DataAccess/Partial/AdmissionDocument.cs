using MON.Shared.Enums;
using MON.Shared.Interfaces;

namespace MON.DataAccess
{
    public partial class AdmissionDocument : IStatus, ICreatable, IUpdatable, IInstitutionNotNullable, IAuditable
    {
        public bool IsDraft => Status == (int)DocumentStatus.Draft;
    }
}
