using MON.Shared.Enums;
using MON.Shared.Interfaces;

namespace MON.DataAccess
{
    public partial class DischargeDocument : IStatus, IInstitution, ICreatable, IUpdatable, IAuditable
    {
        public bool IsDraft => Status == (int)DocumentStatus.Draft;
    }
}