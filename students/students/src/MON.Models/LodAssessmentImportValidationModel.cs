namespace MON.Models
{
    using MON.Shared.ErrorHandling;
    using System.Collections.Generic;
    using System.Linq;

    public class LodAssessmentImportValidationModel
    {
        public List<LodAssessmentImportModel> Models { get; set; }

        public ValidationErrorCollection Errors { get; set; } = new ValidationErrorCollection();

        public bool HasErrors => (Errors != null && Errors.Count > 0) || (Models != null && Models.Any(x => x.HasErrors));
    }
}
