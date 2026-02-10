namespace MON.DataAccess
{
    using MON.Shared.Interfaces;
    using System;

    public partial class InstitutionChange : ICreatable, IUpdatable
    {

        public static InstitutionChange From(int institutionId, short schoolYear)
        {
            return new InstitutionChange()
            {
                InstitutionId = institutionId,
                SchoolYear = schoolYear,
                Version = DateTime.UtcNow
            };
        }
    }
}
