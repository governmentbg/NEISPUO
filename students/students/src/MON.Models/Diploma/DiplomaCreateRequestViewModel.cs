namespace MON.Models.Diploma
{
    using System;
    using System.Collections.Generic;

    public class DiplomaCreateRequestViewModel : DiplomaCreateRequestModel
    {
        public string RequestingInstitutionName { get; set; }
        public string PersonName { get; set; }
        public string PinType { get; set; }
        public string Pin { get; set; }
        public string BasicDocumentName { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifierUsername { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool DiplomaIsSigned { get; set; }
        public bool DiplomaIsCancelled { get; set; }

        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public bool DiplomaIsPublic { get; set; }
        public bool DiplomaIsEditable { get; set; }
    }
}
