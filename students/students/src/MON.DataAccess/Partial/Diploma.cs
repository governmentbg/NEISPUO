using MON.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace MON.DataAccess
{
    public partial class Diploma : ICreatable, IUpdatable, IInstitution, IAuditable, ISchoolYear
    {
        public void ResetSigningAttrs()
        {
            IsSigned = false;
            SignedBySysUserId = null;
            SignedDate = null;
            Signature = null;
        }

        public void ResetIsEditableAttrs()
        {
            IsEditable = false;
            EditableSetBySysUserId = null;
            EditableSetDate = null;
            EditableSetReason = null;
        }
    }
}
