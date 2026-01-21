namespace MON.Report.Service.Student.RelocationDocument
{
    using MON.Models.StudentModels.Lod;
    using MON.Models.StudentModels.StoredProcedures;
    using MON.Report.Model;
    using MON.Report.Service;
    using MON.Services.Implementations;
    using MON.Services.Interfaces;
    using MON.Shared.ErrorHandling;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class RelocationDocument_3_96_6ReportService : ReportService<RelocationDocument_3_96_6ReportService>
    {
        private readonly IRelocationDocumentService _relocationDocumentService;
        private readonly ILodAssessmentService _lodAssessmentService;


        public RelocationDocument_3_96_6ReportService(
            DbReportServiceDependencies<RelocationDocument_3_96_6ReportService> dependencies,
            IRelocationDocumentService relocationDocumentService,
            ILodAssessmentService lodAssessmentService)
            : base(dependencies)
        {
            _relocationDocumentService = relocationDocumentService;
            _lodAssessmentService = lodAssessmentService;
        }
        public override object LoadReport(IDictionary<string, object> parameters)
        {
            int id = GetIdAsInt(parameters);
            parameters.TryGetValue("FilterForCurrentInstitution", out object filterForCurrentInstitutionVal);
            parameters.TryGetValue("FilterForCurrentSchoolBook", out object filterForCurrentSchoolBookVal);
            bool? filterForCurrentInstitution = filterForCurrentInstitutionVal as bool?;
            bool? filterForCurrentSchoolBook = filterForCurrentSchoolBookVal as bool?;

            StudentRelocationDocumentPrintModel model = _db.RelocationDocuments
                .Where(x => x.Id == id)
                .Select(x => new StudentRelocationDocumentPrintModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Pin = $"{x.Person.PersonalIdtypeNavigation.Name} {x.Person.PersonalId}",
                    StudentFullName = $"{x.Person.FirstName} {x.Person.MiddleName} {x.Person.LastName}",
                    StudentNationality = x.Person.Nationality.Name,
                    StudentBirthDate = x.Person.BirthDate,
                    StudentBirthPlaceId = x.Person.BirthPlaceTownId,
                    StudentBirthCity = x.Person.BirthPlaceTown.Name ?? x.Person.BirthPlace,
                    StudentBirthMunicipality = x.Person.BirthPlaceTown.Municipality.Name,
                    StudentBirthRegion = x.Person.BirthPlaceTown.Municipality.Region.Name,
                    StudentBirthPlaceCountry = x.Person.BirthPlaceCountryNavigation.Name,
                    CurrentStudentClassId = x.CurrentStudentClassId,
                    BasicClassId = x.CurrentStudentClass.BasicClassId,
                    BasicClassName = x.CurrentStudentClass.BasicClass.Name,
                    SchoolYearName = x.CurrentStudentClass.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    EduForm = x.CurrentStudentClass.StudentEduForm.Name,
                    ClassType = x.CurrentStudentClass.ClassType.Name,
                    Speciality = x.CurrentStudentClass.StudentSpeciality.Name,
                    Profession = x.CurrentStudentClass.StudentSpeciality.Profession.Name,
                    NoteNumber = x.NoteNumber,
                    NoteDate = x.NoteDate,
                    DischargeDate = x.DischargeDate,
                    SendingInstitutionId = x.SendingInstitutionId,
                    SendingInstitutionName = x.SendingInstitution.Name,
                    SendingInstitutionCity = x.SendingInstitution.Town.Name,
                    SendingInstitutionMunicipality = x.SendingInstitution.Town.Municipality.Name,
                    SendingInstitutionRegion = x.SendingInstitution.Town.Municipality.Region.Name,
                    SendingInstitutionDistrict = x.SendingInstitution.LocalArea.Name,
                    HostingInstitutionName = x.HostInstitution.Name,
                    HostingInstitutionCity = x.HostInstitution.Town.Name,
                    HostingInstitutionMunicipality = x.HostInstitution.Town.Municipality.Name,
                    HostingInstitutionRegion = x.HostInstitution.Town.Municipality.Region.Name,
                    HostingInstitutionDistrict = x.HostInstitution.LocalArea.Name,
                    SchoolYear = x.SendingInstitutionSchoolYear,
                    Evaluations = new List<SubjectEvaluationPrintModel>()
                })
                .SingleOrDefault();

            if (model == null)
            {
                throw new ApiException("Empty model");
            }

            if (model.SchoolYear == default)
            {
                // SendingInstitutionSchoolYear на RelocationDocument-та е било null
                model.SchoolYear = _db.InstitutionSchoolYears
                    .Where(x => x.InstitutionId == model.SendingInstitutionId && x.IsCurrent)
                    .Select(x => x.SchoolYear)
                    .FirstOrDefault();
            }

            List<int> includedBasicClasses = new List<int> { 5, 6 };
            List<StudentLodAssessmentListModel> assessmentsList = _db.VStudentLodAsssessmentLists
                .Where(x => x.PersonId == model.PersonId && x.BasicClassId.HasValue && includedBasicClasses.Contains(x.BasicClassId.Value))
                .Select(x => new StudentLodAssessmentListModel
                {
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    BasicClassId = x.BasicClassId ?? 0,
                    IsSelfEduForm = x.IsSelfEduForm ?? false,
                })
                .ToList();

            foreach (StudentLodAssessmentListModel basciClassAssessment in assessmentsList.OrderByDescending(x => x.BasicClassId))
            {
                List<LodAssessmentCurriculumPartModel> parts = _lodAssessmentService.GetPersonAssessments(model.PersonId, basciClassAssessment.BasicClassId,
                    basciClassAssessment.SchoolYear, basciClassAssessment.IsSelfEduForm, filterForCurrentInstitution,
                    filterForCurrentSchoolBook, CancellationToken.None).Result;
                if (parts != null && parts.Count > 0)
                {
                    foreach (var part in parts)
                    {
                        if (part.SubjectAssessments == null || part.SubjectAssessments.Count == 0) continue;

                        foreach (var subject in part.SubjectAssessments)
                        {
                            SubjectEvaluationPrintModel printModel = model.Evaluations.FirstOrDefault(x => x.CurriculumPartId == part.CurriculumPartId && x.SubjectId == subject.SubjectId && x.SubjectTypeId == subject.SubjectTypeId);
                            if (printModel == null)
                            {
                                printModel = new SubjectEvaluationPrintModel
                                {
                                    SubjectId = subject.SubjectId,
                                    SubjectName = subject.SubjectName,
                                    SubjectTypeId = subject.SubjectTypeId,
                                    SubjectTypeName = subject.SubjectTypeName,
                                    CurriculumPartId = part.CurriculumPartId,
                                    CurriculumPartName = part.CurriculumPartName,
                                    BasicClassId = subject.BasicClassId,
                                    SortOrder = subject.SortOrder,
                                };

                                model.Evaluations.Add(printModel);
                            }

                            List<LodAssessmentGradeCreateModel> annualGrades = subject
                                .AnnualTermAssessments
                                .Concat(subject.FirstRemedialSession)
                                .Concat(subject.SecondRemedialSession)
                                .Concat(subject.AdditionalRemedialSession)
                                .ToList();

                            switch (basciClassAssessment.BasicClassId)
                            {
                                case 6:
                                    printModel.FirstTermEvaluationGrade = subject.FirstTermAssessments.Select(x => new SubjectEvaluationGradeModel
                                    {
                                        GradeId = x.GradeId,
                                        GradeText = x.GradeText,
                                    }).FirstOrDefault();

                                    printModel.SecondTermEvaluationGrade = subject.SecondTermAssessments.Select(x => new SubjectEvaluationGradeModel
                                    {
                                        GradeId = x.GradeId,
                                        GradeText = x.GradeText,
                                    }).FirstOrDefault();

                                    printModel.AnnualEvaluationGrade = GetAnnualEvaluationGrade(subject);
                                    break;
                                case 5:
                                    printModel.FifthGradeEvaluationGrade = annualGrades
                                        .OrderBy(x => x.GradeTypeId).ThenByDescending(x => x.GradeNomSort)
                                        .Select(x => new SubjectEvaluationGradeModel
                                        {
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                        }).FirstOrDefault();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            // Tekuщи оценки от раздел ЗУЧ
            IEnumerable<StudentTermGradesModel> currnetGrades = (_relocationDocumentService.GetStudentCurrentTermGradesAsync(model.Id, filterForCurrentInstitution, filterForCurrentSchoolBook).Result)?.TermGrades ?? new List<StudentTermGradesModel>();

            model.CurrentGrades = new List<SubjectCurrentGradesPrintModel>();
            foreach (var group in currnetGrades.OrderBy(x => x.CurriculumPartID).GroupBy(x => x.CurriculumPartID))
            {
                foreach (var grades in group.GroupBy(x => new { x.SubjectID, x.SubjectTypeID}))
                {
                    model.CurrentGrades.Add(new SubjectCurrentGradesPrintModel
                    {
                        SubjectName = $"{grades?.FirstOrDefault().SubjectName} / {grades?.FirstOrDefault().SubjectTypeName}",
                        FirstTermGrades = string.Join(",", grades.Where(x => x.Term == 1).SelectMany(x => x.Grades).Select(x => x.PrintGradeText)),
                        SecondTermGrades = string.Join(",", grades.Where(x => x.Term == 2).SelectMany(x => x.Grades).Select(x => x.PrintGradeText)),
                        CurriculumPartId = group.Key,
                        CurriculumPartName = grades?.FirstOrDefault()?.CurriculumPartName,
                        SortOrder = grades?.FirstOrDefault()?.SortOrder ?? 0,
                    });
                }
            }

            model.PrincipalName = _db.DirCurrentData
                .Where(x => x.InstitutionId == model.SendingInstitutionId)
                .Select(x => x.NamesDir)
                .FirstOrDefault();

            var absences = _db.VSchoolBooksAbsences
                .Where(x => x.PersonId == model.PersonId && x.InstitutionId == model.SendingInstitutionId
                    && x.SchoolYear == model.SchoolYear && x.Term != null)
                .Select(x => new
                {
                    x.Term,
                    x.Excused,
                    x.Unexcused,
                    x.Late
                })
                .ToList();

            model.Absences = new RelocationDocumentAbsencePrintModel
            {
                AbsencesForValidReasonsFirstTerm = absences.Where(x => x.Term != null && x.Term == 1 && x.Excused != null && x.Excused == true).Count(),
                AbsencesForValidReasonsSecondTerm = absences.Where(x => x.Term != null && x.Term == 2 && x.Excused != null && x.Excused == true).Count(),
                AbsencesForInvalidReasonsFirstTerm = absences.Where(x => x.Term != null && x.Term == 1 && x.Unexcused != null && x.Unexcused == true).Count()
                    + absences.Where(x => x.Term != null && x.Term == 1 && x.Late != null && x.Late == true).Count() / 2f,
                AbsencesForInvalidReasonsSecondTerm = absences.Where(x => x.Term != null && x.Term == 2 && x.Unexcused != null && x.Unexcused == true).Count()
                    + absences.Where(x => x.Term != null && x.Term == 2 && x.Late != null && x.Late == true).Count() / 2f
            };

            model.Sanctions = _db.VSchoolBooksSanctions
                .Where(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear && x.InstitutionId == model.SendingInstitutionId)
                //.Where(x => x.PersonId == 83671237 && x.SchoolYear == 2021 && x.InstitutionId == 1500143) // тест задължително да се изтрие
                .OrderBy(x => x.StartDate)
                .Select(x => new StudentSanctionPrintModel
                {
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToList();

            return model;
        }
    }
}
