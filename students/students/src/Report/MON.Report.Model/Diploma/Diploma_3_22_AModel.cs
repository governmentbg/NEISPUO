namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_22_AModel : DiplomaDuplicateModel
    {
        public Diploma_3_22_AModel(bool dummy) : base(dummy)
        {

        }

        public int? ZIPProfBasicDocumentPartId { get; set; }
        public int? ZIPNoProfBasicDocumentPartId { get; set; }
        public int? SIPBasicDocumentPartId { get; set; }
    }
}
