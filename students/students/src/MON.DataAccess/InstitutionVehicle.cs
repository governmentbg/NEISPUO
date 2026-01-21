using System;

namespace MON.DataAccess
{
    public partial class InstitutionVehicle
    {
        public int InstitutionVehicleId { get; set; }
        public int InstitutionId { get; set; }
        public int AcquisitionWayTypeId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentNum { get; set; }
        public DateTime DocumentDate { get; set; }
        public string ReigstrationNum { get; set; }
        public int ProducedYear { get; set; }
        public int PlaceCountTypeId { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionDetail Institution { get; set; }
    }
}
