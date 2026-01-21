using Microsoft.Extensions.Logging;
using MON.Models.StudentModels.Lod;
using MON.Report.Model;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service
{
    public abstract class ReportService<T> : DbService, IReportService
    {
        protected readonly ILogger _logger;
        protected readonly IUserInfo _userInfo;
        public ReportService(DbReportServiceDependencies<T> dependencies)
            : base(dependencies.Context)
        {
            _logger = dependencies.Logger;
            _userInfo = dependencies.UserInfo;
        }

        public abstract object LoadReport(IDictionary<string, object> parameters);

        /// <summary>
        /// В .trdp файловете най-често е деклариран такъв параметър:
        /// [ReportParameter Name="Id" Type="Integer" /], т.е. името е с главна буква, а типът е числов.
        /// В dictionary-то пристига стойност от тип long, а не int, затова се ползва Convert вместо статичен cast.
        /// </summary>
        public int GetIdAsInt(IDictionary<string, object> parameters)
        {
            return Convert.ToInt32(parameters["Id"]);
        }

        /// <summary>
        /// Годишната оценка трябва е макс от годишната и поправителните сесии
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public SubjectEvaluationGradeModel GetAnnualEvaluationGrade(LodAssessmentCreateModel subject)
        {
            if (subject == null)
            {
                return null;
            }

            return (subject.AnnualTermAssessments ?? new List<LodAssessmentGradeCreateModel>())
                .Where(x => x.GradeId.HasValue)
                .Union(subject.FirstRemedialSession ?? new List<LodAssessmentGradeCreateModel>().Where(x => x.GradeId.HasValue))
                .Union(subject.SecondRemedialSession ?? new List<LodAssessmentGradeCreateModel>().Where(x => x.GradeId.HasValue))
                .Union(subject.FirstRemedialSession ?? new List<LodAssessmentGradeCreateModel>().Where(x => x.GradeId.HasValue))
                .OrderBy(x => x.GradeTypeId).ThenByDescending(x => x.GradeNomSort).ThenByDescending(x => x.GradeId)
                .Select(x => new SubjectEvaluationGradeModel
                {
                    GradeId = x.GradeId,
                    GradeText = x.GradeText,
                })
                .FirstOrDefault();
        }
    }
}
