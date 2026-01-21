namespace MON.Services.Implementations
{
    using MON.Models.Grid;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MON.Shared;
    using System.Linq.Dynamic.Core;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using MON.DataAccess;
    using MON.Shared.Enums;
    using Microsoft.AspNetCore.Http;
    using MON.Models;
    using System.Threading;
    using MON.Services.Extensions;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using System.IO;
    using MON.Models.StudentModels;
    using System.Data;
    using EFCore.BulkExtensions;
    using MON.Models.Enums;
    using System.Text;
    using MON.Models.StudentModels.Class;
    using MON.Shared.Enums.SchoolBooks;

    public class LodAssessmentService : BaseService<LodAssessmentService>, ILodAssessmentService
    {
        private HashSet<int> SearchedCurriculums = new HashSet<int>();
        private HashSet<int> SearchedSubjects = new HashSet<int>();
        private HashSet<string> SearchedSubjectsNames = new HashSet<string>();
        private HashSet<int> SearchedSubjectTypes = new HashSet<int>();
        private HashSet<(string personalId, int personalIdType)> SearchedStudents = new HashSet<(string personalId, int personalIdType)>();

        private Dictionary<(string personalId, int personalIdType), (int personId, int? basicClassId)> Students = new Dictionary<(string PersonalId, int PersonalIdType), (int personId, int? basicClassId)>();

        /// <summary>
        /// Връзка между tmp_CurricID_orig и CurriculumID в inst_year.Curriculum
        /// </summary>
        private Dictionary<int, (int curriculumId, int? curriculumPartId, int? subjectId, int? subjectTypeId, string subjectName, int? horarium)> CurriculumMapper = new Dictionary<int, (int curriculumId, int? curriculumPartId, int? subjectId, int? subjectTypeId, string subjectName, int? horarium)>();

        private Dictionary<int, string> Subjects = new Dictionary<int, string>();
        private Dictionary<int, (string subjectTypeName, int partId)> SubjectTypes = new Dictionary<int, (string subjectTypeName, int partId)>();


        private List<int> AllowedCurriculumParts = new List<int> { 1, 2, 3, 4 };
        private Dictionary<int, int> GradeCodeMapper = new Dictionary<int, int>
        {
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 },
            { 6, 6 },
            { 7, 8 }, // Освободен
            { 8, 7 }, // Неявил се
            { 12, 40 }, // Незадоволителен
            { 13, 41 }, // Среден
            { 14, 42 }, // Добър
            { 15, 43 }, // Много добър
            { 16, 44 }, // Отличен
            { 30, 22 }, // Среща затруднения
            { 31, 21 }, // Справя се
            { 32, 20 }, // Постига изискванията
        };

        private Dictionary<int, int> GradeTypeMapper = new Dictionary<int, int>
        {
            { 0, 3 }, // Годишна
            { 1, 1 }, // Първи срок
            { 2, 2 }, // Втори срок
            { 3, 4 }, // Първа поправителна сесия
            { 4, 5 }, // Втора поправителна сесия
            { 5, 6 }, // Допълнителна поправителна сесия
        };

        private readonly IStudentService _studentService;
        private readonly IInstitutionService _institutionService;

        public LodAssessmentService(DbServiceDependencies<LodAssessmentService> dependencies, IStudentService studentService, IInstitutionService institutionService) : base(dependencies)
        {
            _studentService = studentService;
            _institutionService = institutionService;
        }

        #region Private members

        /// <summary>
        /// Връща групирани оценки. Групират се първо по раздел от учебния план, след което по SubjectId, SubjectTypeId и IsLodSubject
        /// </summary>
        /// <param name="assessmets"></param>
        /// <returns></returns>
        private List<LodAssessmentCurriculumPartModel> GetGroupedByPartLodAssessmentModels(List<VStudentLodAsssessment> assessmets)
        {
            List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart = assessmets
                .Where(x => x.ParentCurriculumId == null) // Изключваме модулите
                .GroupBy(x => x.CurriculumPartId.Value)
                .Select(g => new LodAssessmentCurriculumPartModel
                {
                    PersonId = g.First().PersonId,
                    SchoolYear = g.First().SchoolYear,
                    BasicClassId = g.First().BasicClassId ?? default,
                    CurriculumPartId = g.Key,
                    CurriculumPart = g.First().CurriculumPart,
                    CurriculumPartName = g.First().CurriculumPartName,
                    IsSelfEduForm = g.First().IsSelfEduForm ?? false,
                    SubjectAssessments = g
                        .GroupBy(s => new { SubjectId = s.SubjectId ?? 0, SubjectTypeId = s.SubjectTypeId.Value, s.CurriculumId, s.IsLodSubject })
                        .Select(sg => new LodAssessmentCreateModel
                        {
                            Id = sg.OrderByDescending(x => x.LodAssessmentId).First().LodAssessmentId,
                            SubjectId = sg.Key.SubjectId,
                            SubjectTypeId = sg.Key.SubjectTypeId,
                            SubjectName = sg.First().SubjectName,
                            SubjectTypeName = sg.First().SubjectTypeName,
                            AnnualHorarium = sg.First().AnnualHorarium,
                            FlSubjectId = sg.First().FlSubjectId,
                            FlHorarium = sg.First().FlHorarium,
                            FlSubjectName = sg.First().FlSubjectName,
                            ShowFlSubject = sg.First().FlSubjectId != null,
                            SortOrder = sg.First().SortOrder,
                            IsLodSubject = sg.First().IsLodSubject ?? false,
                            IsModule = false,
                            IsSelfEduForm = sg.First().IsSelfEduForm,
                            CurriculumId = sg.Key.CurriculumId,
                            CurriculumIds = sg.Select(x => x.CurriculumId).ToHashSet(),
                            Categories = sg.Select(x => x.Category).ToHashSet(),
                            StudentClassStatus = sg.First().StudentClassStatus,
                            CurriculumStudentId = sg.First().CurriculumStudentId,
                            FirstTermAssessments = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.FirstTerm)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName= x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            SecondTermAssessments = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.SecondTerm)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName = x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            AnnualTermAssessments = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.Final)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName = x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            FirstRemedialSession = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.FirstRemedial)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName = x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            SecondRemedialSession = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.SecondRemedial)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName = x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            AdditionalRemedialSession = sg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.AdditionalRemedial)
                                .Select(x => new LodAssessmentGradeCreateModel
                                {
                                    Id = x.LodAssessmentGradeId,
                                    GradeId = x.GradeId,
                                    GradeText = x.GradeText,
                                    GradeSource = x.Category,
                                    GradeCategoryId = x.GradeCategoryId,
                                    GradeTypeId = x.GradeTypeId,
                                    DecimalGrade = x.DecimalGrade,
                                    ClassBookName = x.ClassBookName,
                                    GradeNomSort = x.GradeNomSort,
                                    CategoryAbbreviation = x.CategoryAbbreviation,
                                }).ToList(),
                            Modules = assessmets.Where(ma => ma.ParentCurriculumId != null && ma.ParentCurriculumId == sg.First().CurriculumId)
                                .GroupBy(s => new { SubjectId = s.SubjectId ?? 0, SubjectTypeId = s.SubjectTypeId.Value, s.IsLodSubject })
                                .Select(msg => new LodAssessmentCreateModel
                                {
                                    Id = msg.First().LodAssessmentId,
                                    SubjectId = msg.Key.SubjectId,
                                    SubjectTypeId = msg.Key.SubjectTypeId,
                                    SubjectName = msg.First().SubjectName,
                                    SubjectTypeName = msg.First().SubjectTypeName,
                                    AnnualHorarium = msg.First().AnnualHorarium,
                                    FlSubjectId = msg.First().FlSubjectId,
                                    FlHorarium = msg.First().FlHorarium,
                                    FlSubjectName = msg.First().FlSubjectName,
                                    ShowFlSubject = msg.First().FlSubjectId != null,
                                    SortOrder = msg.First().SortOrder,
                                    IsLodSubject = msg.First().IsLodSubject ?? false,
                                    IsModule = true,
                                    IsSelfEduForm = msg.First().IsSelfEduForm,
                                    CurriculumId = msg.First().CurriculumId,
                                    CurriculumIds = msg.Select(x => x.CurriculumId).ToHashSet(),
                                    CurriculumStudentId = msg.First().CurriculumStudentId,
                                    Categories = sg.Select(x => x.Category).ToHashSet(),
                                    FirstTermAssessments = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.FirstTerm)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                    SecondTermAssessments = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.SecondTerm)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                    AnnualTermAssessments = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.Final)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                    FirstRemedialSession = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.FirstRemedial)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                    SecondRemedialSession = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.SecondRemedial)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                    AdditionalRemedialSession = msg.Where(x => x.GradeCategoryId == (int)LodAssessmentGradeCategoryEnum.AdditionalRemedial)
                                        .Select(x => new LodAssessmentGradeCreateModel
                                        {
                                            Id = x.LodAssessmentGradeId,
                                            GradeId = x.GradeId,
                                            GradeText = x.GradeText,
                                            GradeSource = x.Category,
                                            GradeCategoryId = x.GradeCategoryId,
                                            GradeTypeId = x.GradeTypeId,
                                            DecimalGrade = x.DecimalGrade,
                                            ClassBookName = x.ClassBookName,
                                            GradeNomSort = x.GradeNomSort,
                                            CategoryAbbreviation = x.CategoryAbbreviation,
                                        }).ToList(),
                                })
                                .ToList()
                        })
                        .OrderBy(s => s.SubjectTypeId).ThenBy(s => s.SubjectId)
                        .ToList()
                })
                .OrderBy(x => x.CurriculumPartId)
                .ToList();

            return assessmentsGroupsByPart;
        }

        private async Task<List<LodAssessmentCurriculumPartModel>> GetGroupByPartCurriculumsModels(int studentClassId, List<int> allowedCurriculumParts)
        {
            var studentClass = await _context.StudentClasses
                .Where(x => x.Id == studentClassId)
                .Select(x => new
                {
                    x.Id,
                    x.PersonId,
                    x.BasicClassId,
                    x.SchoolYear
                })
                .SingleOrDefaultAsync();

            if (studentClass == null)
            {
                return null;
            }

            var studentCurriculums = await _context.CurriculumStudents
                .Where(x => x.StudentId == studentClassId && x.IsValid == true
                    && x.Curriculum.CurriculumPartId.HasValue
                    && allowedCurriculumParts.Contains(x.Curriculum.CurriculumPartId.Value))
                .Select(x => new CurriculumStudentModel
                {
                    CurriculumStudentId = x.CurriculumStudentId,
                    PersonId = studentClass.PersonId,
                    CurriculumId = x.CurriculumId,
                    ParentCurriculumId = x.Curriculum.ParentCurriculumId, // Ако е != null, значи е модул
                    SubjectId = x.Curriculum.SubjectId,
                    SubjectTypeId = x.Curriculum.SubjectTypeId,
                    CurriculumPartId = x.Curriculum.CurriculumPartId,
                    SubjectName = x.Curriculum.Subject.SubjectName,
                    SubjectNameEn = x.Curriculum.Subject.NameEn,
                    SubjectNameDe = x.Curriculum.Subject.NameDe,
                    SubjectNameFr = x.Curriculum.Subject.NameFr,
                    SubjectTypeName = x.Curriculum.SubjectType.Name,
                    IsFlSubject = x.Curriculum.IsFl ?? false,
                    FlSubjectid = x.Curriculum.FlsubjectId,
                    FlSubjectName = x.Curriculum.Flsubject.Name,
                    CurriculumPart = x.Curriculum.CurriculumPart.Description,
                    CurriculumPartName = x.Curriculum.CurriculumPart.Name,
                    SortOrder = x.Curriculum.SortOrder ?? 1,
                    IsLodSubject = true,
                    IsLoadedFromStudentCurriculum = true,
                    WeeksFirstTerm = x.WeeksFirstTerm,
                    HoursWeeklyFirstTerm = x.HoursWeeklyFirstTerm,
                    WeeksSecondTerm = x.WeeksSecondTerm,
                    HoursWeeklySecondTerm = x.HoursWeeklySecondTerm,
                    CurriculumWeeksFirstTerm = x.Curriculum.WeeksFirstTerm,
                    CurriculumHoursWeeklyFirstTerm = x.Curriculum.HoursWeeklyFirstTerm,
                    CurriculumWeeksSecondTerm = x.Curriculum.WeeksSecondTerm,
                    CurriculumHoursWeeklySecondTerm = x.Curriculum.HoursWeeklySecondTerm,
                })
                .ToListAsync();

            foreach (var c in studentCurriculums)
            {
                c.CalcHorarium();
            }

            await ModulesWithoutProfSubjectFixture(studentCurriculums);


            List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = studentCurriculums
                .Where(x => x.ParentCurriculumId == null && x.CurriculumPartId.HasValue) // Изключваме модулите
                .GroupBy(x => x.CurriculumPartId.Value)
                   .Select(g => new LodAssessmentCurriculumPartModel
                   {
                       PersonId = studentClass.PersonId,
                       SchoolYear = studentClass.SchoolYear,
                       BasicClassId = studentClass.BasicClassId,
                       CurriculumPartId = g.Key,
                       CurriculumPart = g.First().CurriculumPart,
                       CurriculumPartName = g.First().CurriculumPartName,
                       SubjectAssessments = g
                            .OrderBy(s => s.SubjectTypeId).ThenBy(s => s.SubjectId)
                            .GroupBy(s => new { SubjectId = s.SubjectId.Value, SubjectTypeId = s.SubjectTypeId.Value })
                            .Select(sg => new LodAssessmentCreateModel
                            {
                                SubjectId = sg.Key.SubjectId,
                                SubjectTypeId = sg.Key.SubjectTypeId,
                                SubjectName = sg.First().SubjectName,
                                SubjectTypeName = sg.First().SubjectTypeName,
                                AnnualHorarium = sg.Where(x => x.AnnualHorarium.HasValue).Select(x => x.AnnualHorarium.Value).DefaultIfEmpty().Sum(),
                                FlSubjectId = sg.First().FlSubjectid,
                                FlHorarium = sg.First().FlHorarium,
                                ShowFlSubject = sg.First().FlSubjectid != null,
                                FlSubjectName = sg.First().FlSubjectName,
                                SortOrder = sg.First().SortOrder,
                                IsLodSubject = sg.First().IsLodSubject,
                                IsLoadedFromStudentCurriculum = sg.First().IsLoadedFromStudentCurriculum,
                                CurriculumId = sg.First().CurriculumId,
                                CurriculumStudentId = sg.First().CurriculumStudentId?.ToString(),
                                Modules = studentCurriculums.Where(ma => ma.ParentCurriculumId != null && ma.ParentCurriculumId == sg.First().CurriculumId)
                                    .OrderBy(s => s.SubjectTypeId).ThenBy(s => s.SubjectId)
                                    .GroupBy(s => new { SubjectId = s.SubjectId.Value, SubjectTypeId = s.SubjectTypeId.Value })
                                    .Select(msg => new LodAssessmentCreateModel
                                    {
                                        SubjectId = msg.Key.SubjectId,
                                        SubjectTypeId = msg.Key.SubjectTypeId,
                                        SubjectName = msg.First().SubjectName,
                                        SubjectTypeName = msg.First().SubjectTypeName,
                                        AnnualHorarium = msg.Where(x => x.AnnualHorarium.HasValue).Select(x => x.AnnualHorarium.Value).DefaultIfEmpty().Sum(),
                                        FlSubjectId = msg.First().FlSubjectid,
                                        FlHorarium = msg.First().FlHorarium,
                                        ShowFlSubject = msg.First().FlSubjectid != null,
                                        FlSubjectName = msg.First().FlSubjectName,
                                        SortOrder = msg.First().SortOrder,
                                        IsLodSubject = msg.First().IsLodSubject,
                                        IsLoadedFromStudentCurriculum = sg.First().IsLoadedFromStudentCurriculum,
                                        IsModule = true,
                                    })
                                    .ToList()
                            })
                            .ToList()
                   })
                .OrderBy(x => x.CurriculumPartId)
                .ToList();


            return studentCurriculumsByPart;
        }

        /// <summary>
        /// Връща учебния план за ученик, випуск и учебна година. Минава през StudentClass.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="basicClass"></param>
        /// <param name="schoolYear"></param>
        /// <param name="allowedCurriculumParts"></param>
        /// <returns></returns>

        private async Task<List<LodAssessmentCurriculumPartModel>> GetGroupByPartCurriculumsModels(int personId, int basicClass, int schoolYear, List<int> allowedCurriculumParts)
        {
            // Взимаме учебен план
            int studentClassId = await _context.StudentClasses
                .Where(x => x.PersonId == personId && x.BasicClassId == basicClass && x.SchoolYear == schoolYear
                    && x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic)
                .OrderByDescending(x => x.IsCurrent).ThenByDescending(x => x.EnrollmentDate)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            return await GetGroupByPartCurriculumsModels(studentClassId, allowedCurriculumParts);
        }


        /// <summary>
        /// Обединяване на предметите от учебния план с тези на ученика (VIEW [student].[v_StudentLodAsssessments])
        /// </summary>
        /// <param name="studentCurriculumsByPart"></param>
        /// <param name="assessmentsGroupsByPart"></param>
        /// <returns></returns>
        private List<LodAssessmentCurriculumPartModel> MergeAssessmenstsWithCurriculum(List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart,
            List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart)
        {
            if (studentCurriculumsByPart == null && assessmentsGroupsByPart == null)
            {
                return null;
            }

            if (studentCurriculumsByPart == null) return assessmentsGroupsByPart;
            if (assessmentsGroupsByPart == null) return studentCurriculumsByPart;


            // Добавяме оценките към учебния план
            foreach (var partGroup in assessmentsGroupsByPart)
            {
                // Добавяне, ако не съществува, на раздела
                if (!studentCurriculumsByPart.Any(x => x.CurriculumPartId == partGroup.CurriculumPartId))
                {
                    studentCurriculumsByPart.Add(new LodAssessmentCurriculumPartModel
                    {
                        PersonId = partGroup.PersonId,
                        SchoolYear = partGroup.SchoolYear,
                        BasicClassId = partGroup.BasicClassId,
                        CurriculumPartId = partGroup.CurriculumPartId,
                        CurriculumPart = partGroup.CurriculumPart,
                        CurriculumPartName = partGroup.CurriculumPartName
                    });
                }

                LodAssessmentCurriculumPartModel studentCurriculumPart = studentCurriculumsByPart.FirstOrDefault(x => x.CurriculumPartId == partGroup.CurriculumPartId);
                if (studentCurriculumPart == null)
                {
                    // Горното добавяне, ако не съществува, следва да не води до това условие.
                    // За всички случай прескачаме.
                    continue;
                }

                foreach (LodAssessmentCreateModel subjectAssessment in partGroup.SubjectAssessments)
                {
                    // Възможно е да дойдат два предмета със един и същ SubjectId и SubjectTypeId.
                    // В метод GetGroupedByPartLodAssessmentModels оценките се групират  и по IsLodSubject.
                    // т.е. един предмет за оценките въведение в ЛОД-а и един за всички останали(дневник, приравняване и признаване)

                    LodAssessmentCreateModel studentCurriculumSubject = studentCurriculumPart.SubjectAssessments.FirstOrDefault(x => x.SubjectId == subjectAssessment.SubjectId
                        && x.SubjectTypeId == subjectAssessment.SubjectTypeId
                        && x.Index == default
                        //&& x.CurriculumId == subject.CurriculumId
                        );

                    // Добавяне на предмета, ако той не съществува в учебни план
                    if (studentCurriculumSubject == null)
                    {
                        studentCurriculumSubject = new LodAssessmentCreateModel
                        {
                            Id = subjectAssessment.Id,
                            SubjectId = subjectAssessment.SubjectId,
                            SubjectTypeId = subjectAssessment.SubjectTypeId,
                            SubjectName = subjectAssessment.SubjectName,
                            SubjectTypeName = subjectAssessment.SubjectTypeName,
                            SortOrder = subjectAssessment.SortOrder,
                            StudentClassStatus = subjectAssessment.StudentClassStatus,
                        };
                        studentCurriculumPart.SubjectAssessments.Add(studentCurriculumSubject);
                    }


                    // Добавяне на оценките към предмета
                    studentCurriculumSubject.Id = subjectAssessment.Id;
                    studentCurriculumSubject.Index += 1; // Инкрементираме за да знаем, че предмета от учебния план е използван. Служи за справяне с проблема описан в коментара в началото на foreach-а.
                    studentCurriculumSubject.IsLoadedFromStudentCurriculum = false;
                    studentCurriculumSubject.IsLodSubject = subjectAssessment.IsLodSubject;
                    studentCurriculumSubject.IsSelfEduForm = subjectAssessment.IsSelfEduForm;
                    studentCurriculumSubject.IsModule = subjectAssessment.IsModule;
                    studentCurriculumSubject.Categories = subjectAssessment.Categories;
                    studentCurriculumSubject.FirstTermAssessments.AddRange(subjectAssessment.FirstTermAssessments);
                    studentCurriculumSubject.SecondTermAssessments.AddRange(subjectAssessment.SecondTermAssessments);
                    studentCurriculumSubject.AnnualTermAssessments.AddRange(subjectAssessment.AnnualTermAssessments);
                    studentCurriculumSubject.FirstRemedialSession.AddRange(subjectAssessment.FirstRemedialSession);
                    studentCurriculumSubject.SecondRemedialSession.AddRange(subjectAssessment.SecondRemedialSession);
                    studentCurriculumSubject.AdditionalRemedialSession.AddRange(subjectAssessment.AdditionalRemedialSession);
                    studentCurriculumSubject.CurriculumId = subjectAssessment.CurriculumId;
                    studentCurriculumSubject.CurriculumStudentId = subjectAssessment.CurriculumStudentId;

                    if (studentCurriculumSubject.AnnualHorarium != subjectAssessment.AnnualHorarium)
                    {
                        // Ако има разминаване в хорариум-а между учебния план и заредения предмет взимаме този на предмета
                        studentCurriculumSubject.AnnualHorarium = subjectAssessment.AnnualHorarium;
                    }

                    if (studentCurriculumSubject.FlSubjectId != subjectAssessment.FlSubjectId)
                    {
                        // Ако има разминаване в езика за изучаване между учебния план и заредения предмет взимаме този на предмета
                        studentCurriculumSubject.FlSubjectId = subjectAssessment.FlSubjectId;
                        studentCurriculumSubject.FlSubjectName = subjectAssessment.FlSubjectName;
                        studentCurriculumSubject.ShowFlSubject = subjectAssessment.FlSubjectId != null;
                    }

                    if (studentCurriculumSubject.FlHorarium != subjectAssessment.FlHorarium)
                    {
                        // Ако има разминаване в хорариум-а на ЧЕ между учебния план и заредения предмет взимаме този на предмета
                        studentCurriculumSubject.FlHorarium = subjectAssessment.FlHorarium;
                    }

                    studentCurriculumSubject.ShowFlSubject = subjectAssessment.FlSubjectId.HasValue;

                    foreach (LodAssessmentCreateModel module in subjectAssessment.Modules)
                    {
                        // Добавяне, ако не съществува, на модулите
                        if (!studentCurriculumSubject.Modules.Any(x => x.SubjectId == module.SubjectId && x.SubjectTypeId == module.SubjectTypeId))
                        {
                            studentCurriculumSubject.Modules.Add(new LodAssessmentCreateModel
                            {
                                Id = module.Id,
                                SubjectId = module.SubjectId,
                                SubjectTypeId = module.SubjectTypeId,
                                SubjectName = module.SubjectName,
                                SubjectTypeName = module.SubjectTypeName,
                                AnnualHorarium = module.AnnualHorarium,
                                FlSubjectId = module.FlSubjectId,
                                FlHorarium = module.FlHorarium,
                                ShowFlSubject = module.FlSubjectId != null,
                                SortOrder = module.SortOrder,
                                IsLodSubject = module.IsLodSubject,
                                IsModule = module.IsModule,
                                Categories = module.Categories,
                                IsLoadedFromStudentCurriculum = false,
                                IsSelfEduForm = module.IsSelfEduForm,
                            });
                        }

                        // Добавяне на оценките към модула
                        LodAssessmentCreateModel studentCurriculumSubjectModule = studentCurriculumSubject.Modules.First(x => x.SubjectId == module.SubjectId && x.SubjectTypeId == module.SubjectTypeId);
                        studentCurriculumSubjectModule.Id = module.Id;
                        studentCurriculumSubjectModule.IsSelfEduForm = module.IsSelfEduForm;
                        studentCurriculumSubjectModule.IsLoadedFromStudentCurriculum = false;
                        studentCurriculumSubjectModule.FirstTermAssessments.AddRange(module.FirstTermAssessments);
                        studentCurriculumSubjectModule.SecondTermAssessments.AddRange(module.SecondTermAssessments);
                        studentCurriculumSubjectModule.AnnualTermAssessments.AddRange(module.AnnualTermAssessments);
                        studentCurriculumSubjectModule.FirstRemedialSession.AddRange(module.FirstRemedialSession);
                        studentCurriculumSubjectModule.SecondRemedialSession.AddRange(module.SecondRemedialSession);
                        studentCurriculumSubjectModule.AdditionalRemedialSession.AddRange(module.AdditionalRemedialSession);
                        studentCurriculumSubjectModule.CurriculumId = module.CurriculumId;
                        studentCurriculumSubjectModule.CurriculumStudentId = module.CurriculumStudentId;

                        if (studentCurriculumSubjectModule.AnnualHorarium != module.AnnualHorarium)
                        {
                            // Ако има разминаване в хорариум-а между учебния план и заредения предмет взимаме този на предмета
                            studentCurriculumSubjectModule.AnnualHorarium = module.AnnualHorarium;
                        }

                        if (studentCurriculumSubjectModule.FlSubjectId != module.FlSubjectId)
                        {
                            // Ако има разминаване в езика за изучаване между учебния план и заредения предмет взимаме този на предмета
                            studentCurriculumSubjectModule.FlSubjectId = module.FlSubjectId;
                            studentCurriculumSubjectModule.FlSubjectName = module.FlSubjectName;
                        }

                        if (studentCurriculumSubjectModule.FlHorarium != module.FlHorarium)
                        {
                            // Ако има разминаване в хорариум-а на ЧЕ между учебния план и заредения предмет взимаме този на предмета
                            studentCurriculumSubjectModule.FlHorarium = module.FlHorarium;
                        }

                        studentCurriculumSubjectModule.ShowFlSubject = studentCurriculumSubjectModule.FlSubjectId.HasValue;

                    }

                    studentCurriculumSubject.Modules = UnionSubjectModules(studentCurriculumSubject.Modules, subjectAssessment.Modules);
                }

                studentCurriculumPart.SubjectAssessments = studentCurriculumPart.SubjectAssessments
                    .OrderBy(x => x.IsLodSubject)
                    .ThenBy(x => x.SubjectTypeId)
                    .ThenBy(x => x.SubjectId).ToList();
            }

            return studentCurriculumsByPart.OrderBy(x => x.CurriculumPartId).ToList();
        }

        /// <summary>
        ///  Обединяване на модулите на предмет идващи чрез оценките и тези от учебни план.
        /// </summary>
        /// <param name="modules1"></param>
        /// <param name="studentAssessmentSubjectModules"></param>
        /// <exception cref="NotImplementedException"></exception>
        private List<LodAssessmentCreateModel> UnionSubjectModules(List<LodAssessmentCreateModel> studentCurriculumSubjectModules, List<LodAssessmentCreateModel> studentAssessmentSubjectModules)
        {
            return studentAssessmentSubjectModules.Union(
                    studentCurriculumSubjectModules.Where(cm => !studentAssessmentSubjectModules.Any(am => am.SubjectId == cm.SubjectId && am.SubjectTypeId == cm.SubjectTypeId))
                ).OrderBy(x => x.SubjectTypeId).ThenBy(x => x.SubjectId).ToList();

        }

        private async Task<List<LodAssessmentImportModel>> ReadAssessmentFileAsync(IFormFile file, ApiValidationResult validationResult)
        {
            List<string> lines = await file.ReadAsListAsync();
            List<LodAssessmentImportModel> models = ParseAbsenceImportModels(lines, validationResult);



            foreach (LodAssessmentImportModel item in models.Where(x => x.ProfSubjectId.HasValue))
            {
                item.ProfSubjectDisplayText = $"{item.ProfSubjectId} / {models.Where(x => x.SubjectId == item.ProfSubjectId && x.PersonalId == item.PersonalId).Select(x => x.SubjectName).FirstOrDefault()}";
            }

            return models;
        }

        private LodAssessmentImportModel ParseAbsenceImportModel(string line)
        {
            if (line.IsNullOrEmpty()) return null;

            string[] split = line.Split("|");
            LodAssessmentImportModel model = new LodAssessmentImportModel
            {
                PersonalId = split[2],
                SubjectName = split[10],
            };

            if (int.TryParse(split[0], out int institutionId))
            {
                model.InstitutionId = institutionId;
            }

            if (short.TryParse(split[1], out short schooYear))
            {
                model.SchoolYear = schooYear;
            }

            if (int.TryParse(split[3], out int personalIdType))
            {
                model.PersonalIdType = personalIdType;
            }

            if (int.TryParse(split[4], out int curriculumId))
            {
                model.CurriculumId = curriculumId;
            }

            if (int.TryParse(split[5], out int gradeType))
            {
                model.GradeType = gradeType;
            }

            if (int.TryParse(split[6], out int gradeCode))
            {
                model.GradeCode = gradeCode;
            }

            if (int.TryParse(split[7], out int subjectId))
            {
                model.SubjectId = subjectId;
            }

            if (int.TryParse(split[8], out int profSubjectId))
            {
                model.ProfSubjectId = profSubjectId;
            }
            else
            {
                model.ProfSubjectId = null;
            }

            if (int.TryParse(split[9], out int subjectTypeId))
            {
                model.SubjectTypeId = subjectTypeId;
            }

            if (split.Length > 11)
            {
                if (decimal.TryParse(split[11], out decimal profSubjectDecimalGrade))
                {
                    model.ProfSubjectDecimalGrade = profSubjectDecimalGrade;
                }
            }

            if (split.Length > 12)
            {
                if (int.TryParse(split[12], out int basicClassId))
                {
                    model.BasicClassId = basicClassId;
                }
            }

            if (split.Length > 13)
            {
                string str = split[13];
                model.IsNotPresentForm = !string.IsNullOrWhiteSpace(str) && str.Trim() == "1";
            }

            return model;
        }

        private List<LodAssessmentImportModel> ParseAbsenceImportModels(List<string> lines, ApiValidationResult validationResult)
        {
            if (lines.IsNullOrEmpty())
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"Празен файл",
                });

                return null;
            }

            List<LodAssessmentImportModel> studentsAbsences = new List<LodAssessmentImportModel>();
            foreach (string line in lines)
            {
                try
                {
                    LodAssessmentImportModel model = ParseAbsenceImportModel(line);
                    if (model != null)
                    {
                        studentsAbsences.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    var error = new
                    {
                        ex.GetInnerMostException().Message,
                        Action = "ParseLodAssessmentImportModel",
                        Data = line
                    };

                    _logger.LogError(JsonSerializer.Serialize(error));

                    if (ex is IndexOutOfRangeException)
                    {
                        validationResult.Errors.Add(new ValidationError()
                        {
                            Message = "Файлът и редовете в него трябва да отговарят на техническата спецификация за структура и съдържание.Разделителят трябва да е |.",
                        });
                    }

                    return studentsAbsences;
                }
            }

            return studentsAbsences;
        }

        private async Task<int?> GeStudentBasicClass(int personId, int institutionId, int schoolYear, CancellationToken cancellationToken = default)
        {
            int? instType = (await _institutionService.GetInstitutionCache(institutionId))?.InstTypeId;
            IQueryable<StudentClass> studentClassesQuery = _context.StudentClasses
                    .Where(x => x.PersonId == personId && x.InstitutionId == institutionId && x.SchoolYear == schoolYear
                        && x.ClassType.ClassKind == (int)ClassKindEnum.Basic);

            studentClassesQuery = instType switch
            {
                (int)InstitutionTypeEnum.School => studentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds),
                (int)InstitutionTypeEnum.KinderGarden => studentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds),
                (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport => studentClassesQuery.Where(x => x.PositionId == (int)PositionType.StudentOtherInstitution),
                _ => studentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student),
            };

            int? basicClassId = (await studentClassesQuery
                .OrderByDescending(x => x.IsCurrent)
                .ThenBy(x => x.Position)
                .Select(x => new { x.BasicClassId })
                .FirstOrDefaultAsync(cancellationToken))?.BasicClassId;

            if (basicClassId.HasValue)
            {
                return basicClassId;
            }


            // Не сме определили випускът от student.StudentClass. Има случаи на липса на мигрирани данни за предишни години.
            // В този случай ще се опитаме да определим випускът като ровим в sob схемата. 

            IQueryable<VSobStudentClassDetail> sobStudentClassesQuery = _context.VSobStudentClassDetails
                   .Where(x => x.PersonId == personId && x.InstitutionId == institutionId && x.SchoolYear == schoolYear
                       && x.ClassKind == (int)ClassKindEnum.Basic);

            sobStudentClassesQuery = instType switch
            {
                (int)InstitutionTypeEnum.School => sobStudentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds),
                (int)InstitutionTypeEnum.KinderGarden => sobStudentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds),
                (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport => sobStudentClassesQuery.Where(x => x.PositionId == (int)PositionType.StudentOtherInstitution),
                _ => sobStudentClassesQuery.Where(x => x.PositionId == (int)PositionType.Student),
            };

            // Випускът считаме за еднозначно определен когато в sob.StudentClass има един запис или повече, но с еднакъв BasicClassId.

            HashSet<int> basicClassIds = (await sobStudentClassesQuery.Select(x => x.BasicClassId).ToListAsync(cancellationToken))
               .Where(x => x.HasValue).Select(x => x.Value).ToHashSet();

            if (basicClassIds.Count == 1)
            {
                return basicClassIds.First();
            }
            else
            {
                return null;
            }
        }

        private async Task PreLoadReferenceData(List<LodAssessmentImportModel> models,
            int fileNameInstitutionId, int fileNameSchoolYear, CancellationToken cancellationToken = default)
        {
            if (models.IsNullOrEmpty())
            {
                return;
            }

            HashSet<int> curriculumIds = models.Where(x => !SearchedCurriculums.Contains(x.CurriculumId)).Select(x => x.CurriculumId).ToHashSet();
            await LoadCurriculumData(curriculumIds, fileNameInstitutionId, fileNameSchoolYear, cancellationToken);

            HashSet<int> subjectTypeIds = models.Where(x => !SearchedSubjectTypes.Contains(x.SubjectTypeId)).Select(x => x.SubjectTypeId).ToHashSet();
            await LoadSubjectTypeData(subjectTypeIds, cancellationToken);
        }

        private async Task LoadPersonalData(string personalId, int personalIdType, int institutionId, int schoolYear, CancellationToken cancellationToken = default)
        {
            (string personalId, int personalIdType) key = (personalId, personalIdType);
            if (!SearchedStudents.Contains(key))
            {
                var student = await _context.People
                     .Where(x => x.PersonalIdtype == key.personalIdType && x.PersonalId == key.personalId)
                     .Select(x => new { x.PersonId })
                     .FirstOrDefaultAsync(cancellationToken);

                SearchedStudents.Add(key);

                if (student == null)
                {
                    return;
                }

                int? basicClassId = await GeStudentBasicClass(student.PersonId, institutionId, schoolYear, cancellationToken);

                Students.Add(key, (student.PersonId, basicClassId));
            }
        }

        private async Task LoadCurriculumData(HashSet<int> curriculumIds, int fileNameInstitutionId, int fileNameSchoolYear, CancellationToken cancellationToken = default)
        {
            if (curriculumIds.IsNullOrEmpty())
            {
                return;
            }

            var curriculums = (await _context.Curricula
                .Where(x => x.InstitutionId == fileNameInstitutionId && x.SchoolYear == fileNameSchoolYear
                    && x.TmpCurricIdOrig.HasValue && curriculumIds.Contains(x.TmpCurricIdOrig.Value))
                .Select(x => new
                {
                    x.CurriculumId,
                    x.CurriculumPartId,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.Subject.SubjectName,
                    SubjectCurriculumPartId = x.SubjectType.PartId,
                    x.TotalTermHours,
                    OldCurriculumId = x.TmpCurricIdOrig.Value
                })
                .ToListAsync(cancellationToken))
                .GroupBy(x => x.OldCurriculumId)
                .Select(x => x.FirstOrDefaultDynamic());

            foreach (var curriculum in curriculums)
            {
                CurriculumMapper.Add(curriculum.OldCurriculumId, (curriculumId: curriculum.CurriculumId,
                    curriculumPartId: curriculum.SubjectCurriculumPartId, subjectId: curriculum.SubjectId,
                    subjectTypeId: curriculum.SubjectTypeId, subjectName: curriculum.SubjectName, horarium: curriculum.TotalTermHours));

                SearchedCurriculums.Add(curriculum.OldCurriculumId);
            }
        }

        private async Task LoadCurriculumData(int oldCurriculumId, int institutionId, int schoolYear, CancellationToken cancellationToken = default)
        {
            // Зареждаме данни от базата,ако не сме го правили вече
            if (!SearchedCurriculums.Contains(oldCurriculumId))
            {
                var curriculum = await _context.Curricula
                    .Where(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.TmpCurricIdOrig == oldCurriculumId)
                    .Select(x => new
                    {
                        x.CurriculumId,
                        x.CurriculumPartId,
                        x.SubjectId,
                        x.SubjectTypeId,
                        x.Subject.SubjectName,
                        SubjectCurriculumPartId = x.SubjectType.PartId,
                        x.TotalTermHours
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (curriculum != null)
                {
                    CurriculumMapper.Add(oldCurriculumId, (curriculumId: curriculum.CurriculumId, curriculumPartId: curriculum.SubjectCurriculumPartId, subjectId: curriculum.SubjectId, subjectTypeId: curriculum.SubjectTypeId, subjectName: curriculum.SubjectName, horarium: curriculum.TotalTermHours));
                }

                SearchedCurriculums.Add(oldCurriculumId);
            }
        }

        /// <summary>
        /// Зареждане на детайли на предмет
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>«
        private async Task LoadSubjectData(LodAssessmentImportModel model, CancellationToken cancellationToken = default)
        {

            // Предметът вече сме го търсили.
            if (SearchedSubjects.Contains(model.SubjectId)) return;

            // Въпреки задължителността на SubjectId има ситуации, при които не може да се подаде нищо
            // или ако се подаде, то е невалидно. Дневниците са синхронизирали номенклатурата на предметите
            // в началото на учебната година и са давали възможност на потребителите да създават предмети.
            // Сиела ще подават 0 когато нямат SubjectId.


            // Търсим предметър по подаденото ID
            Subject subject = await _context.Subjects
                .Where(x => x.SubjectId == model.SubjectId)
                .SingleOrDefaultAsync(cancellationToken);

            // Предмет с такова ID е намерен
            if (subject != null)
            {
                // Маркираме Id-то като търсено.
                SearchedSubjects.Add(model.SubjectId);

                // Запазваме детайли за предмета
                Subjects.Add(subject.SubjectId, subject.SubjectName);
                return;
            }

            // Търсим предметът по подаденото име.
            string searchSubjectName = model.SubjectName.Trim();
            if (SearchedSubjectsNames.Contains(searchSubjectName)) return;


            subject = await _context.Subjects
                .Where(x => x.SubjectName == model.SubjectName.Trim())
                .OrderByDescending(x => x.IsValid)
                .ThenBy(x => x.SubjectId)
                .FirstOrDefaultAsync(cancellationToken);

            // Маркираме името като търсено.
            SearchedSubjectsNames.Add(searchSubjectName);

            // Предмет с такова име е намерен.
            if (subject != null)
            {
                // Саписваме ID-то му е модела
                model.SubjectId = subject.SubjectId;

                // Запазваме детайли за предмета
                Subjects.Add(subject.SubjectId, subject.SubjectName);
            }
        }

        private async Task LoadSubjectTypeData(HashSet<int> subjectTypeIds, CancellationToken cancellationToken = default)
        {
            if (subjectTypeIds.IsNullOrEmpty())
            {
                return;
            }

            var subjectTypes = (await _context.SubjectTypes
                    .Where(x => subjectTypeIds.Contains(x.SubjectTypeId) && x.PartId != null)
                    .Select(x => new
                    {
                        Id = x.SubjectTypeId,
                        x.Name,
                        PartId = x.PartId.Value
                    })
                    .ToListAsync(cancellationToken))
                    .GroupBy(x => x.Id)
                    .Select(x => x.FirstOrDefault());

            foreach (var subjectType in subjectTypes)
            {
                if (subjectType != null)
                {
                    SubjectTypes.Add(subjectType.Id, (subjectTypeName: subjectType.Name, partId: subjectType.PartId));
                }

                SearchedSubjectTypes.Add(subjectType.Id);

            }
        }

        private async Task LoadSubjectTypeData(int subjectTypeId, CancellationToken cancellationToken = default)
        {
            if (!SearchedSubjectTypes.Contains(subjectTypeId))
            {
                var subjectType = await _context.SubjectTypes
                    .Where(x => x.SubjectTypeId == subjectTypeId && x.PartId != null)
                    .Select(x => new
                    {
                        x.Name,
                        PartId = x.PartId.Value
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (subjectType != null)
                {
                    SubjectTypes.Add(subjectTypeId, (subjectTypeName: subjectType.Name, partId: subjectType.PartId));
                }

                SearchedSubjectTypes.Add(subjectTypeId);
            }
        }

        private async Task ValidatePersonalId(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            await LoadPersonalData(model.PersonalId, model.PersonalIdType, model.InstitutionId, model.SchoolYear, cancellationToken);

            if (!Students.TryGetValue((model.PersonalId, model.PersonalIdType), out (int personId, int? basicClassId) student))
            {
                var error = new ValidationError
                {
                    Message = $"В НЕИСПУО не съществува лице с идентификатор: {model.PersonalId} и вид на идентификатора: {model.PersonalIdType}! Моля да създадете нов ученик и след това да импортирате файла. Не е необходимо да записвате ученика в институцията!",
                    ControlID = $"Ред: {rowNum}",
                    ID = LodAssessmentImportErrorCodeEnum.PersonError.ToString()
                };

                model.Errors.Add(error);
            }
            else
            {
                model.PersonId = student.personId;

                if (!model.BasicClassId.HasValue)
                {
                    if (student.basicClassId.HasValue)
                    {
                        model.BasicClassId = student.basicClassId.Value;
                    }
                    else
                    {
                        var error = new ValidationError
                        {
                            Message = $"За лице с идентификатор {model.PersonalId} и вид на идентификатора: {model.PersonalIdType} в НЕИСПУО липсва инфромация за записване в паралелка/група на институция {model.InstitutionId} за {model.SchoolYear} учебна година! Поради това, випускът не може да бъде определен. Моля, използвайте функционалността за премахване на грешен запис от импортния файл или коригирайте съдържанието на файла (ръчно или с помощта на екипа по поддръжка на приложението, от което е генериран файла.",
                            ControlID = $"Ред: {rowNum}",
                            ID = LodAssessmentImportErrorCodeEnum.ClassError.ToString()
                        };

                        model.Errors.Add(error);
                    }
                }
            }
        }

        private async Task ValidateCurriculum(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            await LoadCurriculumData(model.CurriculumId, model.InstitutionId, model.SchoolYear, cancellationToken);

            if (CurriculumMapper.TryGetValue(model.CurriculumId, out (int curriculumId, int? curriculumPartId, int? subjectId, int? subjectTypeId, string subjectName, int? horarium) curriculum))
            {
                model.Horarium = curriculum.horarium;

                // Намерили сме данни за inst_year.Curriculum.
                // Трябва да сравним SubjectId-то и SubjectType-а
                if (curriculum.subjectId.HasValue && curriculum.subjectId.Value != model.SubjectId)
                {
                    model.SubjectId = curriculum.subjectId.Value;
                    //var error = new ValidationError
                    //{
                    //    Message = $"Кодът на предмета ({model.SubjectId}), подаден във файла, не съответства с кода на предмета от учебния план в НЕИСПУО ({curriculum.subjectId}).",
                    //    ControlID = $"Ред: {rowNum}",
                    //};

                    //model.Errors.Add(error);
                }

                if (curriculum.subjectTypeId.HasValue && curriculum.subjectTypeId.Value != model.SubjectTypeId)
                {
                    model.SubjectTypeId = curriculum.subjectTypeId.Value;
                    //var error = new ValidationError
                    //{
                    //    Message = $"Начинът на изучаване на предмета ({model.SubjectTypeId}), подаден във файла, не съответства с този на предмета от учебния план в НЕИСПУО ({curriculum.subjectTypeId}).",
                    //    ControlID = $"Ред: {rowNum}",
                    //};

                    //model.Errors.Add(error);
                }

                if (!curriculum.subjectName.IsNullOrEmpty() && curriculum.subjectName != model.SubjectName)
                {
                    model.SubjectName = curriculum.subjectName;
                    //var error = new ValidationError
                    //{
                    //    Message = $"Името на предмета ({model.SubjectName}), подаден във файла, не съответства с името на предмета от учебния план в НЕИСПУО ({curriculum.subjectName}).",
                    //    ControlID = $"Ред: {rowNum}",
                    //};

                    //model.Errors.Add(error);
                }
            }
        }

        private async Task ValidateSubject(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default  )
        {
            await LoadSubjectData(model, cancellationToken);

            if (Subjects.TryGetValue(model.SubjectId, out string subjectName))
            {
                if (!subjectName.Equals(model.SubjectName ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    var error = new ValidationError
                    {
                        Message = $"Наименованието на предмета ({model.SubjectName}), подадено във файла, не съответства на наименованието на предмета в НЕИСПУО ({subjectName}).",
                        ControlID = $"Ред: {rowNum}",
                        ID = LodAssessmentImportErrorCodeEnum.SubjectError.ToString(),
                    };

                    model.Errors.Add(error);
                }
            }
            else
            {
                var error = new ValidationError
                {
                    Message = $"В НЕИСПУО не е намерен предмет с код {model.SubjectId}",
                    ControlID = $"Ред: {rowNum}",
                    ID = LodAssessmentImportErrorCodeEnum.SubjectError.ToString(),
                };

                model.Errors.Add(error);
            }
        }

        private void ValidateGradeCode(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            if (!GradeCodeMapper.ContainsKey(model.GradeCode))
            {
                var error = new ValidationError
                {
                    Message = $"Невалиден код на оценка {model.GradeCode}",
                    ControlID = $"Ред: {rowNum}",
                    ID = LodAssessmentImportErrorCodeEnum.GradeError.ToString()
                };

                model.Errors.Add(error);
            }
        }

        private void ValidateGradeType(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            if (!GradeTypeMapper.ContainsKey(model.GradeType))
            {
                var error = new ValidationError
                {
                    Message = $"Невалиден вид на оценка {model.GradeType}",
                    ControlID = $"Ред: {rowNum}",
                    ID = LodAssessmentImportErrorCodeEnum.GradeError.ToString()
                };

                model.Errors.Add(error);
            }
        }

        private async Task ValidateSubjectType(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            await LoadSubjectTypeData(model.SubjectTypeId, cancellationToken);

            if (SubjectTypes.TryGetValue(model.SubjectTypeId, out (string subjectTypeName, int partId) subjectType))
            {
                model.CurriculumPartId = subjectType.partId;
            }
            else
            {
                var error = new ValidationError
                {
                    Message = $"В НЕИСПУО не е намерен начин на изучаване на предмет с код {model.SubjectTypeId}",
                    ControlID = $"Ред: {rowNum}",
                    ID = LodAssessmentImportErrorCodeEnum.SubjectTypeError.ToString()
                };

                model.Errors.Add(error);
            }
        }

        private Task ValidateProfSubject(LodAssessmentImportModel model, int rowNum, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private (int fileNameInstitutionId, int fileNameSchoolYear) ValidateImportFile(string file, ApiValidationResult validationResult)
        {
            int fileNameInstitutionId = 0;
            int fileNameSchoolYear = 0;
            string fileName = Path.GetFileNameWithoutExtension(file);
            string[] split = fileName.Split("_", StringSplitOptions.RemoveEmptyEntries);
            if (split.Length <= 0 || !int.TryParse(split[0], out fileNameInstitutionId))
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = "Невалиден код по НЕИСПУО в името на файла",
                    ID = LodAssessmentImportErrorCodeEnum.InstitutionError.ToString()
                });
            }

            if (split.Length <= 1 || !int.TryParse(split[1], out fileNameSchoolYear))
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = "Невалидна учебна година в името на файла",
                    ID = LodAssessmentImportErrorCodeEnum.SchoolYearError.ToString()
                });
            }

            if (_userInfo.InstitutionID != fileNameInstitutionId)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = "Невалиден код по НЕИСПУО в името на файла. Нямате право да импортирате оценки от името на друга институция.",
                    ID = LodAssessmentImportErrorCodeEnum.InstitutionError.ToString()
                });
            }

            return (fileNameInstitutionId, fileNameSchoolYear);
        }



        private async Task ValidateImport(List<LodAssessmentImportModel> models,
            int fileNameInstitutionId, int fileNameSchoolYear,
            ApiValidationResult validationResult, CancellationToken cancellationToken = default)
        {

            await PreLoadReferenceData(models, fileNameInstitutionId, fileNameSchoolYear, cancellationToken);

            for (int i = 0; i < models.Count; i++)
            {
                LodAssessmentImportModel model = models[i];
                int rowNum = i + 1;

                if (model.InstitutionId != fileNameInstitutionId)
                {
                    var error = new ValidationError
                    {
                        Message = $"Кодът по НЕИСПУО ({model.InstitutionId}) се различва от този в името на файла ({fileNameInstitutionId})",
                        ID = LodAssessmentImportErrorCodeEnum.InstitutionError.ToString()
                    };

                    model.Errors.Add(error);
                    validationResult.Errors.Add(error);
                }

                if (model.InstitutionId != _userInfo.InstitutionID)
                {
                    var error = new ValidationError
                    {
                        Message = "Невалиден код по НЕИСПУО",
                        ID = LodAssessmentImportErrorCodeEnum.InstitutionError.ToString()
                    };

                    model.Errors.Add(error);
                    validationResult.Errors.Add(error);
                }

                if (model.SchoolYear != fileNameSchoolYear)
                {
                    var error = new ValidationError
                    {
                        Message = $"Учебната година ({model.SchoolYear}) се различва от тази в името на файла ({fileNameSchoolYear})",
                        ID = LodAssessmentImportErrorCodeEnum.SchoolYearError.ToString()
                    };

                    model.Errors.Add(error);
                    validationResult.Errors.Add(error);
                }

                await ValidatePersonalId(model, rowNum, cancellationToken);
                await ValidateCurriculum(model, rowNum, cancellationToken);
                await ValidateSubject(model, rowNum, cancellationToken);
                await ValidateSubjectType(model, rowNum, cancellationToken);
                ValidateGradeCode(model, rowNum, cancellationToken);
                ValidateGradeType(model, rowNum, cancellationToken);

                await ValidateProfSubject(model, rowNum, cancellationToken);

                validationResult.Errors.AddRange(model.Errors);
            }
        }

        private List<LodAssessment> GetMainSubjects(List<LodAssessmentImportModel> models, List<int> profSubjectTypes, DateTime utcNow)
        {
            List<LodAssessment> subjects = new List<LodAssessment>();
            var t = new int[4, 4];
            if (!models.Any())
            {
                return subjects;
            }

            IEnumerable<LodAssessmentImportModel> mainSubjects = models.Where(x => !x.ProfSubjectId.HasValue && x.BasicClassId.HasValue
                        && !profSubjectTypes.Contains(x.SubjectTypeId));
            IEnumerable<LodAssessmentImportModel> profSubjects = models.Where(x => !x.ProfSubjectId.HasValue && x.BasicClassId.HasValue
                && profSubjectTypes.Contains(x.SubjectTypeId));

            var groups = mainSubjects
                .Union(profSubjects)
                .GroupBy(x => new
                {
                    x.PersonId,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.CurriculumPartId,
                    x.InstitutionId,
                    x.SchoolYear,
                    BasicClassId = x.BasicClassId.Value
                });

            foreach (var g in groups)
            {
                subjects.Add(new LodAssessment
                {
                    PersonId = g.Key.PersonId,
                    SubjectId = g.Key.SubjectId,
                    SubjectTypeId = g.Key.SubjectTypeId,
                    CurriculumPartId = g.Key.CurriculumPartId,
                    SchoolYear = g.Key.SchoolYear,
                    InstitutionId = g.Key.InstitutionId,
                    BasicClassId = g.Key.BasicClassId,
                    IsSelfEduForm = g.FirstOrDefault()?.IsNotPresentForm ?? false,
                    ParentId = null,
                    Position = 1,
                    IsImported = true,
                    CreatedBySysUserId = _userInfo.SysUserID,
                    CreateDate = utcNow,
                    Horarium = g.FirstOrDefault()?.Horarium,
                    LodAssessmentGrades = g.Select(x => new LodAssessmentGrade
                    {
                        GradeCategoryId = GradeTypeMapper[x.GradeType],
                        GradeId = GradeCodeMapper[x.GradeCode],
                        DecimalGrade = x.ProfSubjectDecimalGrade,
                        CreatedBySysUserId = _userInfo.SysUserID,
                        CreateDate = utcNow,
                    }).ToList()
                });
            }

            return subjects;
        }

        private async Task<List<LodAssessment>> GetModuleSubjects(List<LodAssessmentImportModel> models, List<int> profSubjectTypes, DateTime utcNow)
        {
            List<LodAssessment> subjects = new List<LodAssessment>();
            IEnumerable<LodAssessmentImportModel> modules = models.Where(x => x.ProfSubjectId.HasValue && x.BasicClassId.HasValue);

            if (!modules.Any())
            {
                return subjects;
            }

            int institutionId = modules.Select(x => x.InstitutionId).FirstOrDefault();
            int schoolYear = modules.Select(x => x.SchoolYear).FirstOrDefault();

            var existingProfSubjects = await _context
                .LodAssessments
                .Where(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.IsImported
                    && profSubjectTypes.Contains(x.SubjectTypeId)
                    && x.SubjectId < 200)
                .Select(x => new
                {
                    x.Id,
                    x.PersonId,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.CurriculumPartId,
                    x.InstitutionId,
                    x.SchoolYear,
                    x.BasicClassId
                })
                .ToListAsync();

            var modulesGroups = modules
                .GroupBy(x => new
                {
                    x.PersonId,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.CurriculumPartId,
                    x.InstitutionId,
                    x.SchoolYear,
                    BasicClassId = x.BasicClassId.Value,
                    ProfSubjectId = x.ProfSubjectId.Value
                });

            foreach (var g in modulesGroups)
            {
                subjects.Add(new LodAssessment
                {
                    PersonId = g.Key.PersonId,
                    SubjectId = g.Key.SubjectId,
                    SubjectTypeId = g.Key.SubjectTypeId,
                    CurriculumPartId = g.Key.CurriculumPartId,
                    SchoolYear = g.Key.SchoolYear,
                    InstitutionId = g.Key.InstitutionId,
                    BasicClassId = g.Key.BasicClassId,
                    IsSelfEduForm = g.FirstOrDefault()?.IsNotPresentForm ?? false,
                    Horarium = g.FirstOrDefault()?.Horarium,
                    ParentId = existingProfSubjects.FirstOrDefault(x => x.PersonId == g.Key.PersonId
                        && x.InstitutionId == g.Key.InstitutionId
                        && x.SchoolYear == g.Key.SchoolYear
                        && x.SubjectId == g.Key.ProfSubjectId
                        && x.CurriculumPartId == g.Key.CurriculumPartId
                        && x.BasicClassId == g.Key.BasicClassId)?.Id,
                    Position = 1,
                    IsImported = true,
                    CreatedBySysUserId = _userInfo.SysUserID,
                    CreateDate = utcNow,
                    LodAssessmentGrades = g.Select(x => new LodAssessmentGrade
                    {
                        GradeCategoryId = GradeTypeMapper[x.GradeType],
                        GradeId = GradeCodeMapper[x.GradeCode],
                        CreatedBySysUserId = _userInfo.SysUserID,
                        CreateDate = utcNow,
                    }).ToList()
                });
            }

            return subjects;

        }

        /// <summary>
        /// При взимането на оценки от дневника изниква следния казус:
        /// Взимамат се само срочни и годишни оценки. Има срочни оценки по дадени модули,
        /// но липсва годишна оценка по проф.предмет, а само такава се въвежда.
        /// В този случай не се показват модулите.
        /// </summary>
        /// <param name="asssessments"></param>
        /// <returns></returns>
        public async Task ModulesWithoutProfSubjectFixture(List<VStudentLodAsssessment> asssessments)
        {
            if (asssessments.IsNullOrEmpty())
            {
                return;
            }

            List<VStudentLodAsssessment> modulesWithoutProfSubject = asssessments.Where(x => !x.ReassessmentTypeId.HasValue
                && x.ParentCurriculumId.HasValue
                && !asssessments.Any(s => s.PersonId == x.PersonId && s.CurriculumId == x.ParentCurriculumId))
                .ToList();

            if (modulesWithoutProfSubject.Count == 0)
            {
                return;
            }

            List<VStudentLodAsssessment> profSubjects = asssessments.Where(x => !x.ParentCurriculumId.HasValue
             && GlobalConstants.SubjectTypesOfProfileSubject.Contains(x.SubjectTypeId)).ToList();

            HashSet<int> curriculumIds = modulesWithoutProfSubject.Select(x => x.ParentCurriculumId.Value).ToHashSet();

            var curriculums = await _context.Curricula
                .Where(x => curriculumIds.Contains(x.CurriculumId))
                .Select(x => new
                {
                    x.CurriculumId,
                    x.CurriculumPartId,
                    CurriculumPart = x.CurriculumPart.Description,
                    CurriculumPartName = x.CurriculumPart.Name,
                    x.SchoolYear,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.Subject.SubjectName,
                    SubjectNameEn = x.Subject.NameEn,
                    SubjectNameDe = x.Subject.NameDe,
                    SubjectNameFr = x.Subject.NameFr,
                    SubjectTypeName = x.SubjectType.Name,
                    x.SortOrder,
                    x.FlsubjectId,
                    FlSubjectName = x.Flsubject.Name,
                    x.InstitutionId
                })
                .ToListAsync();

            foreach (VStudentLodAsssessment module in modulesWithoutProfSubject)
            {
                // Търсим профилиращ предмет
                var curriculum = curriculums.FirstOrDefault(x => x.CurriculumId == module.ParentCurriculumId);
                if (curriculum == null)
                {
                    continue;
                }

                var profSubject = profSubjects.FirstOrDefault(x => x.SubjectTypeId == curriculum.SubjectTypeId && x.SubjectId == curriculum.SubjectId);
                if (profSubject != null)
                {
                    module.ParentCurriculumId = profSubject.CurriculumId;
                    continue;
                }

                asssessments.Add(new VStudentLodAsssessment
                {
                    LodAssessmentId = null,
                    LodAssessmentGradeId = null,
                    PersonId = module.PersonId,
                    SchoolYear = module.SchoolYear,
                    ClassId = module.ClassId,
                    ClassBookName = module.ClassBookName,
                    CurriculumId = curriculum.CurriculumId,
                    ParentCurriculumId = null,
                    CurriculumPartId = curriculum.CurriculumPartId,
                    SubjectId = curriculum.SubjectId,
                    SubjectTypeId = curriculum.SubjectTypeId,
                    SubjectName = curriculum.SubjectName,
                    SubjectNameEn = curriculum.SubjectNameEn,
                    SubjectNameDe = curriculum.SubjectNameDe,
                    SubjectNameFr = curriculum.SubjectNameFr,
                    SubjectTypeName = curriculum.SubjectTypeName,
                    CurriculumPart = curriculum.CurriculumPart,
                    CurriculumPartName = curriculum.CurriculumPartName,
                    BasicClassId = module.BasicClassId,
                    BasicClassName = module.BasicClassName,
                    BasicClassRomeName = module.BasicClassRomeName,
                    InstitutionId = curriculum.InstitutionId,
                    Type = null,
                    TypeName = "",
                    CategoryName = "",
                    Term = 1,
                    DecimalGrade = null,
                    QualitativeGrade = null,
                    SpecialGrade = null,
                    OtherGrade = null,
                    GradeId = null,
                    GradeText = "",
                    GradeTypeId = null,
                    Category = module.Category,
                    ClassBookId = module.ClassBookId,
                    EqualizationId = null,
                    RecognitionId = null,
                    SortOrder = curriculum.SortOrder ?? 1,
                    FlSubjectId = curriculum.FlsubjectId,
                    FlSubjectName = curriculum.FlSubjectName,
                    FlHorarium = null,
                    GradeCategoryId = 1,
                    IsSelfEduForm = false,
                    IsLodSubject = false,
                });
            }

        }

        private async Task ModulesWithoutProfSubjectFixture(List<CurriculumStudentModel> curriculumStudents)
        {
            if (curriculumStudents.IsNullOrEmpty())
            {
                return;
            }

            List<CurriculumStudentModel> modulesWithoutProfSubject = curriculumStudents.Where(x => x.ParentCurriculumId.HasValue
                && !curriculumStudents.Any(s => s.PersonId == x.PersonId && s.CurriculumId == x.ParentCurriculumId))
                .ToList();

            if (modulesWithoutProfSubject.Count == 0)
            {
                return;
            }

            List<CurriculumStudentModel> profSubjects = curriculumStudents.Where(x => !x.ParentCurriculumId.HasValue
             && GlobalConstants.SubjectTypesOfProfileSubject.Contains(x.SubjectTypeId)).ToList();

            HashSet<int> curriculumIds = modulesWithoutProfSubject.Select(x => x.ParentCurriculumId.Value).ToHashSet();

            var curriculums = await _context.Curricula
                .Where(x => curriculumIds.Contains(x.CurriculumId))
                .Select(x => new
                {
                    x.CurriculumId,
                    x.CurriculumPartId,
                    CurriculumPart = x.CurriculumPart.Description,
                    CurriculumPartName = x.CurriculumPart.Name,
                    x.SchoolYear,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.Subject.SubjectName,
                    SubjectNameEn = x.Subject.NameEn,
                    SubjectNameDe = x.Subject.NameDe,
                    SubjectNameFr = x.Subject.NameFr,
                    SubjectTypeName = x.SubjectType.Name,
                    x.SortOrder,
                    x.FlsubjectId,
                    FlSubjectName = x.Flsubject.Name,
                    x.InstitutionId
                })
                .ToListAsync();

            foreach (var module in modulesWithoutProfSubject)
            {
                var curriculum = curriculums.FirstOrDefault(x => x.CurriculumId == module.ParentCurriculumId);
                if (curriculum == null || curriculumStudents.Any(x => x.CurriculumId == curriculum.CurriculumId))
                {
                    continue;
                }
   
                curriculumStudents.Add(new CurriculumStudentModel
                {                  
                    PersonId = module.PersonId,
                    CurriculumId = curriculum.CurriculumId,
                    ParentCurriculumId = null,
                    SubjectId = curriculum.SubjectId,
                    SubjectTypeId = curriculum.SubjectTypeId,
                    CurriculumPartId = curriculum.CurriculumPartId,
                    SubjectName = curriculum.SubjectName,
                    SubjectNameEn = curriculum.SubjectNameEn,
                    SubjectNameDe = curriculum.SubjectNameDe,
                    SubjectNameFr = curriculum.SubjectNameFr,
                    SubjectTypeName = curriculum.SubjectTypeName,
                    FlSubjectid = curriculum.FlsubjectId,
                    FlSubjectName = curriculum.FlSubjectName,
                    FlHorarium = null,
                    CurriculumPart = curriculum.CurriculumPart,
                    CurriculumPartName = curriculum.CurriculumPartName,
                    SortOrder = curriculum.SortOrder ?? 1,
                    IsLodSubject = false,
                    IsLoadedFromStudentCurriculum = true
                });
            }

        }

        private void CalcHorrariumAndAvgGrade(bool isSelfEduForm, List<LodAssessmentCurriculumPartModel> models)
        {
            foreach (var part in models)
            {
                part.IsSelfEduForm = isSelfEduForm;

                foreach (var subject in part.SubjectAssessments)
                {
                    // Хорариумът на проф.предмет е сума от хор. на неговите модули.
                    if (subject.Modules != null && subject.Modules.Any(x => x.AnnualHorarium.HasValue))
                    {
                        int? horrarium = subject.Modules.Where(x => x.AnnualHorarium.HasValue).DefaultIfEmpty().Sum(x => x.AnnualHorarium);
                        if (horrarium > 0)
                        {
                            subject.AnnualHorarium = horrarium;
                        }
                    }

                    // Изчисляване на годишна оценка на профилиращ предмет, ако такава липсва
                    if (subject.AnnualTermAssessments.IsNullOrEmpty() && !subject.Modules.IsNullOrEmpty()
                        && subject.Modules.Any(x => x.AnnualTermAssessments != null && x.AnnualTermAssessments.Any(s => s.DecimalGrade.HasValue && s.DecimalGrade.Value >= 2 && s.DecimalGrade.Value <= 6)))
                    {

                        List<decimal> modulesAnnualGrades = GetModulesAnnualAssessments(subject.Modules);
                        decimal? avgGrade = modulesAnnualGrades.DefaultIfEmpty().Average();

                        if (avgGrade.HasValue)
                        {
                            decimal decimalGrade = Math.Round(avgGrade.Value, 2, MidpointRounding.AwayFromZero);
                            subject.AnnualTermAssessments.Add(new LodAssessmentGradeCreateModel
                            {
                                GradeText = decimalGrade.ToString("0.00"),
                                DecimalGrade = decimalGrade,
                                GradeSource = "Изчислена",
                                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.Final
                            });
                        }
                    }
                }
            }
        }

        private List<decimal> GetModulesAnnualAssessments(List<LodAssessmentCreateModel> modules)
        {
            List<decimal> grades = new List<decimal>();
            if (modules.IsNullOrEmpty())
            {
                return grades;
            }

            foreach (var module in modules)
            {
                List<decimal> moduleGrades = new List<decimal>();
                if (!module.AnnualTermAssessments.IsNullOrEmpty())
                {
                    moduleGrades.AddRange(module.AnnualTermAssessments.Where(x => x.DecimalGrade.HasValue && x.DecimalGrade.Value >= 2 && x.DecimalGrade.Value <= 6).Select(x => x.DecimalGrade.Value));        
                }

                if (!module.FirstRemedialSession.IsNullOrEmpty())
                {
                    moduleGrades.AddRange(module.FirstRemedialSession.Where(x => x.DecimalGrade.HasValue && x.DecimalGrade.Value >= 2 && x.DecimalGrade.Value <= 6).Select(x => x.DecimalGrade.Value));
                }

                if (!module.SecondRemedialSession.IsNullOrEmpty())
                {
                    moduleGrades.AddRange(module.SecondRemedialSession.Where(x => x.DecimalGrade.HasValue && x.DecimalGrade.Value >= 2 && x.DecimalGrade.Value <= 6).Select(x => x.DecimalGrade.Value));
                }

                if (!moduleGrades.IsNullOrEmpty())
                {
                    grades.Add(moduleGrades.Max());
                }
            }

            return grades;
        }

        private void GroupEqualSubjects(List<LodAssessmentCurriculumPartModel> models)
        {
            if (models.IsNullOrEmpty())
            {
                return;
            }

            foreach (var part in models)
            {
                if (part.SubjectAssessments.IsNullOrEmpty())
                {
                    continue;
                }

                List<LodAssessmentCreateModel> groupedSubjects = new List<LodAssessmentCreateModel>();

                foreach (var group in part.SubjectAssessments.GroupBy(x => new { x.SubjectId, x.SubjectTypeId, x.IsLodSubject, x.IsLoadedFromStudentCurriculum, x.AnnualHorarium }))
                {
                    // В предишната стъпка групираме предметите по CurriculumId. 
                    // Има ситуации, в които ученик се мести за втория срок от един клас в друг.
                    // Директорите пишат срочна оценка е за единия дневник (пр. 7б клас) и за новия (пр. 7в клас).
                    // В този случай трябва да покажем само едната срочна оценка, по възможност от текущич дневник, който се определя от StudentClassStatus == (int)StudentClassStatus.Enrolled.
                    // Затова групираме по StudentClassStatus, подреждаме и взимаме първия. StudentClassStatus.Enrolled = 1 и разчитаме да вземем него.

                    LodAssessmentCreateModel subject = group.First();
                    // Оригинален код, понякога изпуска оценките от първия срок
                    //subject.FirstTermAssessments = group
                    //    .GroupBy(x => x.StudentClassStatus)
                    //    .OrderBy(x => x.Key)
                    //    .First()
                    //    .SelectMany(x => x.FirstTermAssessments)
                    //    .ToList();

                    var firstTermAssessments = group
                        .Where(x => !x.FirstTermAssessments.IsNullOrEmpty())
                        .GroupBy(x => x.StudentClassStatus)
                        .OrderBy(x => x.Key)
                        .FirstOrDefault();
                    subject.FirstTermAssessments = firstTermAssessments != null ? firstTermAssessments
                        .SelectMany(x => x.FirstTermAssessments).ToList() : new List<LodAssessmentGradeCreateModel>();
                    // Оригинален код, понякога изпуска оценките от втория срок
                    //subject.SecondTermAssessments = group
                    //    .GroupBy(x => x.StudentClassStatus)
                    //    .OrderBy(x => x.Key)
                    //    .First()
                    //    .SelectMany(x => x.SecondTermAssessments).ToList();

                    // Разглеждаме само тези елементи, в които има оценка за втория срок
                    var secondTermAssessments = group
                        .Where(x => !x.SecondTermAssessments.IsNullOrEmpty())
                        .GroupBy(x => x.StudentClassStatus)
                        .OrderBy(x => x.Key)
                        .FirstOrDefault();
                    subject.SecondTermAssessments = secondTermAssessments != null ? secondTermAssessments
                        .SelectMany(x => x.SecondTermAssessments).ToList() : new List<LodAssessmentGradeCreateModel>();
                    subject.AnnualTermAssessments = group.SelectMany(x => x.AnnualTermAssessments).ToList();
                    subject.FirstRemedialSession = group.SelectMany(x => x.FirstRemedialSession).ToList();
                    subject.SecondRemedialSession = group.SelectMany(x => x.SecondRemedialSession).ToList();
                    subject.AdditionalRemedialSession = group.SelectMany(x => x.AdditionalRemedialSession).ToList();
                    subject.Modules = group.SelectMany(x => x.Modules).ToList();
                    subject.LodAssessmentGrades = group.SelectMany(x => x.LodAssessmentGrades).ToList();
                    subject.LodAssessmentChildren = group.SelectMany(x => x.LodAssessmentChildren).ToList();

                    groupedSubjects.Add(subject);
                }

                part.SubjectAssessments = groupedSubjects;
            }
        }

        #endregion

        public async Task<IPagedList<StudentLodAssessmentListModel>> List(LodEvaluationListInput input, CancellationToken cancellationToken = default)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(LodEvaluationListInput)));
            }

            if (!await _authorizationService.HasPermissionForStudent(input.PersonId, DefaultPermissions.PermissionNameForStudentEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VStudentLodAsssessmentList> query = _context.VStudentLodAsssessmentLists
                .Where(x => x.PersonId == input.PersonId && x.BasicClassId.HasValue);

            if(!input.SchoolYears.IsNullOrEmpty())
            {
                query = query.Where(x => input.SchoolYears.Contains(x.SchoolYear));
            }

            IQueryable<StudentLodAssessmentListModel> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.BasicClassRomeName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.Categories.Contains(input.Filter))
                .Select(x => new StudentLodAssessmentListModel
                {
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    SchoolYear = x.SchoolYear,
                    BasicClassId = x.BasicClassId ?? 0,
                    IsSelfEduForm = x.IsSelfEduForm ?? false,
                    BasicClassName = x.BasicClassRomeName,
                    BasicClassDescription = x.BasicClassName,
                    SchooYearName = x.SchoolYearName,
                    Categories = x.Categories,
                    GradesCount = x.GradesCount ?? 0,
                    LodAssessmentsCount = x.LodAssessmentsCount ?? 0
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, BasicClassId asc, IsSelfEduForm asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<StudentLodAssessmentListModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<LodAssessmentImportListModel>> ListImported(LodAssessmentListInput input, CancellationToken cancellationToken = default)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(LodAssessmentListInput)));
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var query = _context.VLoadAssessmentLists
                .Where(x => x.InstitutionId == _userInfo.InstitutionID && x.IsImported);

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            List<int> profSubjectTypes = GlobalConstants.SubjectTypesOfProfileSubject.Select(x => x.Value).ToList();

            IQueryable<LodAssessmentImportListModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.SubjectName.Contains(input.Filter)
                   || predicate.GradeName.Contains(input.Filter))
                .Select(x => new LodAssessmentImportListModel
                {
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    BasicClassId = x.BasicClassId,
                    SubjectId = x.SubjectId,
                    SubjectTypeId = x.SubjectTypeId,
                    FullName = x.FullName,
                    Pin = x.Pin,
                    PinTypeName = x.PinTypeName,
                    SubjectName = x.SubjectName,
                    SubjectTypeName = x.SubjectTypeName,
                    CurriculumPartName = x.CurriculumPartName,
                    SchoolYearName = x.SchoolYearName,
                    GradeCategoryName = x.GradeCategoryName,
                    GradeId = x.GradeId,
                    GradeName = x.GradeName,
                    DecimalGrade = x.DecimalGrade,
                    IsModule = x.ParentId != null,
                    IsProfSubject = profSubjectTypes.Contains(x.SubjectTypeId),
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, BasicClassId asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<LodAssessmentImportListModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.Uid = Guid.NewGuid().ToString(); // Използва се за ключ на грида
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<List<LodAssessmentCurriculumPartModel>> GetPersonAssessments(int personId, int basicClass, int schoolYear, bool isSelfEduForm, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEvaluationRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            //// Взимаме предметите от учебния план.
            //// Те трябва да присъстват, дори и да липсват оценки за тях.
            List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = isSelfEduForm
                ? new List<LodAssessmentCurriculumPartModel>()
                : (await GetGroupByPartCurriculumsModels(personId, basicClass, schoolYear, AllowedCurriculumParts) ?? new List<LodAssessmentCurriculumPartModel>());

            //List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = new List<LodAssessmentCurriculumPartModel>();

            // Взимаме оценките от дневник, лод, признаване и приравняване
            IQueryable<VStudentLodAsssessment> asssessmentsQuery = _context.VStudentLodAsssessments
                .Where(x => x.PersonId == personId
                    && x.CurriculumPartId.HasValue
                    && (x.SubjectId.HasValue || x.BasicClassId == 1)
                    && x.SubjectTypeId.HasValue
                    && x.BasicClassId.HasValue
                    && x.BasicClassId == basicClass && x.SchoolYear == schoolYear
                    && (x.GradeId != null || x.IsLodSubject != null)
                    && x.IsSelfEduForm == isSelfEduForm
                    && AllowedCurriculumParts.Contains(x.CurriculumPartId.Value));

            if (filterForCurrentInstitution ?? false)
            {
                asssessmentsQuery = asssessmentsQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }

            if (filterForCurrentSchoolBook ?? false)
            {
                asssessmentsQuery = asssessmentsQuery.Where(x => x.StudentClassStatus == (int)StudentClassStatus.Enrolled);
            }

            List<VStudentLodAsssessment> assessments = basicClass == 1 // За първи клас се интересуваме само от общата оценка
                ? await asssessmentsQuery.Where(x => x.Type == (int)GradeTypeEnum.Final).ToListAsync(cancellationToken)
                : await asssessmentsQuery.ToListAsync(cancellationToken);

            await ModulesWithoutProfSubjectFixture(assessments);

            // Групираме оценките по раздел
            List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart = GetGroupedByPartLodAssessmentModels(assessments);

            // Обединяваме оценките с учебния план
            List<LodAssessmentCurriculumPartModel> models = MergeAssessmenstsWithCurriculum(studentCurriculumsByPart, assessmentsGroupsByPart);

            CalcHorrariumAndAvgGrade(isSelfEduForm, models);

            GroupEqualSubjects(models);

            return models;
        }

        public async Task<List<LodAssessmentCurriculumPartModel>> GetPersonCurrentAssessments(int personId, int basicClass, int schoolYear,
            bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            //// Взимаме предметите от учебния план.
            //// Те трябва да присъстват, дори и да липсват оценки за тях.
            List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = await GetGroupByPartCurriculumsModels(personId, basicClass, schoolYear, AllowedCurriculumParts) ?? new List<LodAssessmentCurriculumPartModel>();

            //List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = new List<LodAssessmentCurriculumPartModel>();

            // Взимаме оценките от дневник, лод, признаване и приравняване
            IQueryable<VStudentLodCurrentAsssessment> asssessmentsQuery = _context.VStudentLodCurrentAsssessments
                .Where(x => x.PersonId == personId
                    && x.CurriculumPartId.HasValue
                    && (x.SubjectId.HasValue || x.BasicClassId == 1)
                    && x.SubjectTypeId.HasValue
                    && x.BasicClassId.HasValue
                    && x.BasicClassId == basicClass
                    && x.SchoolYear == schoolYear
                    && AllowedCurriculumParts.Contains(x.CurriculumPartId.Value));

            if (filterForCurrentInstitution ?? false)
            {
                asssessmentsQuery = asssessmentsQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }

            if (filterForCurrentSchoolBook ?? false)
            {
                asssessmentsQuery = asssessmentsQuery.Where(x => x.StudentClassStatus == (int)StudentClassStatus.Enrolled);
            }

            List<VStudentLodAsssessment> assessments = (await asssessmentsQuery.ToListAsync(cancellationToken)).Select(x => (VStudentLodAsssessment)x).ToList();

            await ModulesWithoutProfSubjectFixture(assessments);

            // Групираме оценките по раздел
            List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart = GetGroupedByPartLodAssessmentModels(assessments);

            // Обединяваме оценките с учебния план
            List<LodAssessmentCurriculumPartModel> models = MergeAssessmenstsWithCurriculum(studentCurriculumsByPart, assessmentsGroupsByPart);

            CalcHorrariumAndAvgGrade(false, models);

            GroupEqualSubjects(models);

            return models;

            // Закоментирано на 01.02.2024 rnikolov
            // Обсъдено с Марги.

            //if (basicClass == 1)
            //{
            //    // За първи клас се интересуваме само от общата оценка
            //    LodAssessmentCurriculumPartModel firstGradeModel = new LodAssessmentCurriculumPartModel()
            //    {
            //        BasicClassId = basicClass,
            //        SubjectAssessments = assessments.Select(i => new LodAssessmentCreateModel()
            //        {
            //            SubjectId = i.SubjectId ?? 0,
            //            SubjectName = i.SubjectName,
            //            SubjectTypeName = i.SubjectTypeName,
            //            SubjectTypeId = i.SubjectTypeId.Value,
            //            PersonId = i.PersonId,
            //            SchoolYear = i.SchoolYear,
            //            BasicClassId = i.BasicClassId,
            //            AnnualTermAssessments = new List<LodAssessmentGradeCreateModel> { new LodAssessmentGradeCreateModel()
            //            {
            //                DecimalGrade = i.DecimalGrade,
            //                GradeText = i.GradeText,
            //                GradeId = i.GradeId,
            //                GradeCategoryId = i.GradeCategoryId,
            //                GradeSource = i.Category
            //            } }
            //        }).ToList()
            //    };

            //    return new List<LodAssessmentCurriculumPartModel> { firstGradeModel };

            //}
            //else
            //{
            //    // Групираме оценките по раздел
            //    List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart = GetGroupedByPartLodAssessmentModels(assessments);

            //    // Обединяваме оценките с учебния план
            //    List<LodAssessmentCurriculumPartModel> models = MergeAssessmenstsWithCurriculum(studentCurriculumsByPart, assessmentsGroupsByPart);

            //    CalcHorrariumAndAvgGrade(false, models);

            //    GroupEqualSubjects(models);

            //    return models;
            //}
        }

        public async Task<List<LodAssessmentCurriculumPartModel>> GetStudentClassCurriculum(int studentClassId, CancellationToken cancellationToken = default)
        {
            var studentClass = await _context.StudentClasses
                .Where(x => x.Id == studentClassId)
                .Select(x => new
                {
                    x.PersonId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (!await _authorizationService.HasPermissionForStudent(studentClass?.PersonId ?? 0, DefaultPermissions.PermissionNameForStudentEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Взимаме предметите от учебния план.
            // Те трябва да присъстват, дори и да липсват оценки за тях.
            List<LodAssessmentCurriculumPartModel> studentCurriculumsByPart = (await GetGroupByPartCurriculumsModels(studentClassId, AllowedCurriculumParts)) ?? new List<LodAssessmentCurriculumPartModel>();


            List<VStudentLodAsssessment> assessmets = new List<VStudentLodAsssessment>();

            // Групираме оценките по раздел
            List<LodAssessmentCurriculumPartModel> assessmentsGroupsByPart = GetGroupedByPartLodAssessmentModels(assessmets);

            // Обединяваме оценките с учебния план
            List<LodAssessmentCurriculumPartModel> models = MergeAssessmenstsWithCurriculum(studentCurriculumsByPart, assessmentsGroupsByPart);

            return models;
        }

        public async Task Import(IFormFile file)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (file == null)
            {
                throw new ApiException(Messages.EmptyFileError, 400);
            }

            ApiValidationResult validationResult = new ApiValidationResult();

            (int fileNameInstitutionId, int fileNameSchoolYear) = ValidateImportFile(file.FileName, validationResult);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            List<LodAssessmentImportModel> models = await ReadAssessmentFileAsync(file, validationResult);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            await ValidateImport(models, fileNameInstitutionId, fileNameSchoolYear, validationResult);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.LodAssessments
                    .Where(x => x.InstitutionId == fileNameInstitutionId && x.SchoolYear == fileNameSchoolYear && x.IsImported)
                    .BatchDeleteAsync();

                DateTime utcNow = DateTime.UtcNow;
                List<int> profSubjectTypes = GlobalConstants.SubjectTypesOfProfileSubject.Select(x => x.Value).ToList();

                List<LodAssessment> subjectsToAdd = GetMainSubjects(models, profSubjectTypes, utcNow);
                await _context.BulkInsertAsync(subjectsToAdd, bulkConfig: new BulkConfig { IncludeGraph = true }, progress: null, type: null);

                List<LodAssessment> modulesToAdd = await GetModuleSubjects(models, profSubjectTypes, utcNow);
                await _context.BulkInsertAsync(modulesToAdd, bulkConfig: new BulkConfig { IncludeGraph = true }, progress: null, type: null);

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task<LodAssessmentImportValidationModel> ValidateImport(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (file == null)
            {
                throw new ApiException(Messages.EmptyFileError, 400);
            }

            ApiValidationResult validationResult = new ApiValidationResult();
            LodAssessmentImportValidationModel validationModel = new LodAssessmentImportValidationModel();

            (int fileNameInstitutionId, int fileNameSchoolYear) = ValidateImportFile(file.FileName, validationResult);
            validationModel.Models = await ReadAssessmentFileAsync(file, validationResult);

            if (validationResult.HasErrors)
            {
                validationModel.Errors.AddRange(validationResult.Errors);
            }

            await ValidateImport(validationModel.Models, fileNameInstitutionId, fileNameSchoolYear, validationResult, cancellationToken);

            return validationModel;
        }

        public async Task CreateOrUpdate(List<LodAssessmentCurriculumPartModel> model)
        {
            if (model.IsNullOrEmpty())
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model.FirstOrDefault()?.PersonId ?? 0, DefaultPermissions.PermissionNameForStudentEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int personId = model.First().PersonId;
            short schoolYear = model.First().SchoolYear;
            int basicClassId = model.First().BasicClassId;
            bool isSelfEduForm = model.First().IsSelfEduForm;
            int institutionId = _userInfo.InstitutionID.Value;

            //изтриваме старите
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                List<LodAssessment> existingLodAssessments = await _context.LodAssessments
                    .Include(x => x.LodAssessmentGrades)
                    .Include(x => x.Parent)
                    .Include(x => x.InverseParent)
                    .Where(x => x.PersonId == personId && x.BasicClassId == basicClassId && x.SchoolYear == schoolYear && x.IsSelfEduForm == isSelfEduForm)
                    .ToListAsync();

                List<LodAssessmentCreateModel> createModels = model.OrderBy(x => x.CurriculumPartId)
                    .SelectMany(x => x.GetLodAssessmentCreateModels())
                    .ToList();

                // За изтриване (основни предмети,а не модули). Модулите се управляват в LodAssessment.FromModel или LodAssessment.Update
                List<LodAssessment> toDelete = existingLodAssessments
                    .Where(x => !x.ParentId.HasValue && !createModels.Any(m => m.Id.HasValue && m.Id == x.Id))
                    .ToList();
                _context.LodAssessments.RemoveRange(toDelete);

                foreach (LodAssessmentCreateModel createModel in createModels)
                {
                    LodAssessment entity = existingLodAssessments.FirstOrDefault(x => x.Id == createModel.Id || (x.SubjectId == createModel.SubjectId && x.SubjectTypeId == createModel.SubjectTypeId));

                    if (entity == null)
                    {
                        // За добавяне. Добавят се само предмети, които са добавени от потребителя (не са заредени от учебния план),
                        // имат въведени оценки или имат модули с въведение оценки
                        if (createModel.IsLoadedFromStudentCurriculum != true
                            || createModel.LodAssessmentGrades.Any(x => x.GradeId.HasValue)
                            || createModel.LodAssessmentChildren.Any(x => x.IsLoadedFromStudentCurriculum != true || x.LodAssessmentGrades.Any(g => g.GradeId.HasValue)))
                        {
                            _context.LodAssessments.Add(LodAssessment.FromModel(createModel, institutionId));
                        }
                    }
                    else
                    {
                        // За редакция
                        entity.Update(createModel, institutionId);
                    }
                }

                _context.LodAssessments.RemoveRange(toDelete);

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task<List<ClassGroupDropdownViewModel>> GetMainStudentClasses(int personId, CancellationToken cancellationToken)
        {
            IList<StudentClassViewModel> studentClasses = await _studentService.GetMainStudentClasses(personId, includeIsnotPresentForm: true);

            var studentLodAssessments = await _context.LodAssessments
                .Where(x => x.PersonId == personId)
                .GroupBy(x => new { x.SchoolYear, x.BasicClassId, x.IsSelfEduForm })
                .Select(x => new
                {
                    x.Key,
                })
                .ToDictionaryAsync(x => x.Key, cancellationToken);

            return studentClasses
                .Where(x => !studentLodAssessments.ContainsKey(new { x.SchoolYear, x.BasicClassId, IsSelfEduForm = x.IsNotPresentForm ?? false }))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new ClassGroupDropdownViewModel
                {
                    Value = x.Id ?? 0,
                    Text = $"{x.ClassGroupName} / {x.BasicClassRomeName} / {x.InstitutionId} - {x.InstitutionAbbreviation}",
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    BasicClassId = x.BasicClassId,
                    IsNotPresentForm = x.IsNotPresentForm ?? false,
                    IsLodFinalized = x.IsLodFinalized
                })
                .ToList();

        }

        public async Task Delete(int personId, int basicClass, int schoolYear, bool isSelfEduForm, int institutionId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await _context.LodAssessments
                .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear && x.BasicClassId == basicClass && x.IsSelfEduForm == isSelfEduForm && x.InstitutionId == _userInfo.InstitutionID.Value)
                .BatchDeleteAsync(cancellationToken);
        }

        public byte[] ExportFile(LodAssessmentImportValidationModel model)
        {
            if (model == null || model.Models.IsNullOrEmpty())
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            var sb = new StringBuilder();
            foreach (LodAssessmentImportModel item in model.Models)
            {
                sb.AppendLine(item.ToFileLine());
            }

            Encoding utfEncoding = Encoding.UTF8;

            byte[] bytes = utfEncoding.GetBytes(sb.ToString());
            return bytes;
        }
    }
}
