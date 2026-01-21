namespace MON.Report.Service.Student
{
    using MON.Report.Model;
    using System.Collections.Generic;
    using System.Linq;

    internal class NoteReportService : ReportService<NoteReportService>
    {
        public NoteReportService(DbReportServiceDependencies<NoteReportService> dependencies)
            : base(dependencies)
        {

        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            int id = GetIdAsInt(parameters);

            return _db.Notes.Where(n => n.Id == id)
               .Select(n => new NotePrintModel
               {
                   Content = n.Content,
                   IssueDate = n.IssueDate.ToString("MM/dd/yyyy"),
                   SchoolYear = n.SchoolYear,
                   Title = n.Title,
                   InstitutionName = n.InstitutionSchoolYear.Name,
                   NoteNumber = 123123125523,

               })
               .FirstOrDefault();
        }
    }
}
