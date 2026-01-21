namespace MON.Report.Service
{
    using MON.Models.StudentModels.Lod;
    using MON.Report.Model;
    using System.Collections.Generic;

    public interface IReportService
    {
        object LoadReport(IDictionary<string, object> parameters);

        int GetIdAsInt(IDictionary<string, object> parameters);
        SubjectEvaluationGradeModel GetAnnualEvaluationGrade(LodAssessmentCreateModel subject);
    }
}
