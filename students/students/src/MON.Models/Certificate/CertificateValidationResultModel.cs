namespace MON.Models.Certificate
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CertificateValidationResultModel
    {
        public bool IsValid { get; set; }
        public string Errors { get; set; }
    }
}
