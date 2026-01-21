namespace MON.Models.Refugee
{
    using System;
    using System.Collections.Generic;

    public class ApplicationModel
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string ApplicantFullName { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }
        public DropdownViewModel PersonalIdTypeModel { get; set; }
        public int NationalityId { get; set; }
        public int TownId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int GuardianType { get; set; }
        public int Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public List<ApplicationChildModel> Children { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
    }


}
