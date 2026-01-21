namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.PreSchool;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Enums;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text;
    using System.Threading.Tasks;
    using MON.Shared;
    using System.Threading;
    using DocumentFormat.OpenXml.Math;

    public class PreSchoolEvaluationService : BaseService<PreSchoolEvaluationService>, IPreSchoolEvaluationService
    {
        private readonly IInstitutionService _institutionService;

        // Български език и литература; 80
        // Математика; 81
        // Околен свят; 82
        // Изобразително изкуство; 83
        // Музика; 84
        // Конструиране и технологии; 85
        // Физическа култура; 86
        private readonly List<int> PreSchoolSubjects = new List<int>() {
            (int)PreSchoolSubjectEnum.BEL,
            (int)PreSchoolSubjectEnum.Math,
            (int)PreSchoolSubjectEnum.Environment,
            (int)PreSchoolSubjectEnum.Music,
            (int)PreSchoolSubjectEnum.Technology,
            (int)PreSchoolSubjectEnum.PhysicalEducation,
            (int)PreSchoolSubjectEnum.Art };


        public PreSchoolEvaluationService(DbServiceDependencies<PreSchoolEvaluationService> dependencies,
            IInstitutionService institutionService)
           : base(dependencies)
        {
            _institutionService = institutionService;
        }

        public async Task<IPagedList<PreSchoolEvaluationViewModel>> List(PreSchoolEvalListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(input.PersonId ?? int.MinValue, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<PreSchoolEvaluationViewModel> listQuery = _context.PreSchoolEvaluations
                .Where(x => x.PersonId == input.PersonId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.BasicClass.Name.Contains(input.Filter)
                   || predicate.Subject.SubjectName.Contains(input.Filter))
                .Select(x => new PreSchoolEvaluationViewModel
                {
                    Id = x.Id,
                    BasicClassId = x.BasicClassId,
                    BasicClass = x.BasicClass.Name,
                    SubjectId = x.SubjectId,
                    Subject = x.Subject.SubjectName,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    StartOfYearEvaluation = x.StartOfYearEvaluation,
                    EndOfYearEvaluation = x.EndOfYearEvaluation,
                    EnteredStartOfYearEvaluation = !string.IsNullOrWhiteSpace(x.StartOfYearEvaluation),
                    EnteredEndOfYearEvaluation = !string.IsNullOrWhiteSpace(x.EndOfYearEvaluation)
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "BasicClassId asc, SchoolYear desc, SubjectId asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            IList<PreSchoolEvaluationViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<PreSchoolEvaluationViewModel> GetEvalById(int id, CancellationToken cancellationToken)
        {
            var model = await _context.PreSchoolEvaluations
                .Where(x => x.Id == id)
                .Select(x => new PreSchoolEvaluationViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BasicClassId = x.BasicClassId,
                    BasicClass = x.BasicClass.Name,
                    SubjectId = x.SubjectId,
                    Subject = x.Subject.SubjectName,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    StartOfYearEvaluation = x.StartOfYearEvaluation,
                    EndOfYearEvaluation = x.EndOfYearEvaluation
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<PreSchoolEvaluationViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var preSchoolEvaluations = await (
                from i in _context.PreSchoolEvaluations
                where i.PersonId == personId
                select new PreSchoolEvaluationViewModel()
                {
                    Id = i.Id,
                    BasicClassId = i.BasicClassId,
                    BasicClass = i.BasicClass.RomeName,
                    SchoolYear = i.SchoolYear,
                    PersonId = i.PersonId,
                    SubjectId = i.SubjectId.Value,
                    Subject = i.Subject.SubjectName ?? "Обобщени данни",
                    StartOfYearEvaluation = i.StartOfYearEvaluation,
                    EndOfYearEvaluation = i.EndOfYearEvaluation
                }).ToListAsync(cancellationToken);

            return preSchoolEvaluations;
        }

        public async Task<PreSchoolReadinessModel> GetReadinessForFirstGradeAsync(int personId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            PreSchoolReadinessModel model = await (
                from i in _context.PreSchoolReadinesses
                where i.PersonId == personId
                select new PreSchoolReadinessModel()
                {
                    Id = i.Id,
                    PersonId = i.PersonId,
                    Contents = i.Contents
                }).FirstOrDefaultAsync(cancellationToken);

            return model;
        }

        public async Task CreateForBasicClass(PreSchoolEvaluationModel model)
        {
            if (model == null) throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            short currentSchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);
            var exisitngSubjectIds = await _context.PreSchoolEvaluations
                .Where(x => x.PersonId == model.PersonId
                    && x.BasicClassId == model.BasicClassId
                    && PreSchoolSubjects.Contains(x.SubjectId ?? 0)
                    && x.SchoolYear == currentSchoolYear)
                .Select(x => x.SubjectId)
                .ToListAsync();

            _context.PreSchoolEvaluations.AddRange(PreSchoolSubjects.Where(x => !exisitngSubjectIds.Contains(x)).Select(x => new PreSchoolEvaluation()
            {
                PersonId = model.PersonId,
                SubjectId = x,
                BasicClassId = model.BasicClassId,
                SchoolYear = currentSchoolYear
            }));

            await SaveAsync();
        }

        public async Task ImportFromSchoolBook(int personId, int basicClassId, short? schoolYear)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<PreSchoolEvaluation> evaluations = await _context.PreSchoolEvaluations
               .Where(i => i.PersonId == personId
                   && i.BasicClassId == basicClassId)
               .ToListAsync();

            #region Възможнoст за въвеждане на повече от една оценка в PreSchoolEvaluation за един BasicClass #1237

            // Детето може да е в една възрастова група повече от една учебна година.
            // Освен подадената ще вземем и учебните години от PreSchoolEvaluations

            List<short> schoolYears = new List<short>();
            if (schoolYear.HasValue)
            {
                schoolYears.Add(schoolYear.Value);
            }

            if (evaluations.Count > 0)
            {
                schoolYears.AddRange(evaluations.Select(x => x.SchoolYear).GroupBy(x => x).Select(x => x.Key));
            }

            #endregion


            var dataFromSchoolBook = await (from x in _context.PgResults
                                            where x.PersonId == personId
                                                 && (
                                                 (x.ClassBook.BasicClassId != null && x.ClassBook.BasicClassId == basicClassId)
                                                 || (x.ClassBook.BasicClassId == null && schoolYears.Contains(x.SchoolYear))
                                                 )
                                            select new
                                            {
                                                x.StartSchoolYearResult,
                                                x.EndSchoolYearResult,
                                                x.SubjectId,
                                                x.SchoolYear,
                                            })
                    .ToListAsync();

            if (dataFromSchoolBook.IsNullOrEmpty())
            {
                throw new ApiException(Messages.SchoolBookMissingDataError);
            }

            foreach (var schoolBooEvalGroup in dataFromSchoolBook.GroupBy(x => new { x.SubjectId, x.SchoolYear }))
            {
                PreSchoolEvaluation eval = evaluations.FirstOrDefault(x => x.SubjectId == schoolBooEvalGroup.Key.SubjectId && x.SchoolYear == schoolBooEvalGroup.Key.SchoolYear);
                if (eval == null)
                {
                    eval = new PreSchoolEvaluation
                    {
                        PersonId = personId,
                        BasicClassId = basicClassId,
                        SubjectId = schoolBooEvalGroup.Key.SubjectId,
                        SchoolYear = schoolBooEvalGroup.Key.SchoolYear,
                    };

                    _context.PreSchoolEvaluations.Add(eval);
                }

                StringBuilder startYear = new StringBuilder();
                StringBuilder endYear = new StringBuilder();
                foreach (var schoolBookEval in schoolBooEvalGroup)
                {
                    if (!schoolBookEval.StartSchoolYearResult.IsNullOrWhiteSpace())
                    {
                        startYear.AppendLine(schoolBookEval.StartSchoolYearResult);
                    }

                    if (!schoolBookEval.EndSchoolYearResult.IsNullOrWhiteSpace())
                    {
                        endYear.AppendLine(schoolBookEval.EndSchoolYearResult);
                    }
                }

                eval.StartOfYearEvaluation = startYear.Length > 0 ? startYear.ToString() : null;
                eval.EndOfYearEvaluation = endYear.Length > 0 ? endYear.ToString() : null;
            }

            await SaveAsync();
        }

        public async Task CreateReadinessForFirstGrade(PreSchoolEvaluationModel model)
        {
            if (model == null) throw new ApiException(Messages.EmptyModelError);

            int personId = model.PersonId;
            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
            var personEvaluations = await (
                    from i in _context.PreSchoolEvaluations
                    where i.PersonId == personId
                    select new
                    {
                        i.BasicClassId,
                        i.BasicClass.SortOrd,
                        i.SchoolYear,
                        Grade = i.BasicClass.RomeName,
                        i.SubjectId,
                        Subject = i.Subject.SubjectName,
                        EndYear = i.EndOfYearEvaluation
                    }).
                   ToListAsync();

            var evaluationsByBasicClass = (
                from i in personEvaluations
                group i by new { i.SortOrd, i.SchoolYear } into g
                select new
                {
                    SortAndSchoolYear = g.Key,
                    Evaluations = g.ToList()
                });

            // Изтегляме оценките само за най-горната подготвителна група, ако такъв съществува
            var evaluations = evaluationsByBasicClass
                .OrderByDescending(i => i.SortAndSchoolYear.SortOrd).ThenByDescending(i => i.SortAndSchoolYear.SchoolYear)
                .FirstOrDefault()?
                .Evaluations
                .OrderBy(i => i.Grade)
                .ThenBy(i => i.SubjectId);

            PreSchoolReadiness preSchoolReadiness = await _context.PreSchoolReadinesses.FirstOrDefaultAsync(i => i.PersonId == personId);
            if (preSchoolReadiness == null)
            {
                preSchoolReadiness = new PreSchoolReadiness()
                {
                    PersonId = personId
                };
                StringBuilder firstGradeReadiness = new StringBuilder();
                foreach (var evaluation in evaluations)
                {
                    firstGradeReadiness.Append($"{evaluation.Subject}: {evaluation.EndYear}\r\n");
                }
                preSchoolReadiness.Contents = $"{firstGradeReadiness}";

                _context.PreSchoolReadinesses.Add(preSchoolReadiness);
                await SaveAsync();
            }
        }

        public async Task Update(PreSchoolEvaluationModel model)
        {
            if (model == null) throw new ApiException(Messages.EmptyModelError);

            PreSchoolEvaluation entity = await _context.PreSchoolEvaluations.SingleOrDefaultAsync(i => i.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                entity.StartOfYearEvaluation = model.StartOfYearEvaluation;
                entity.EndOfYearEvaluation = model.EndOfYearEvaluation;
                entity.SchoolYear = model.SchoolYear;
                await SaveAsync();
            }
        }

        public async Task UpdateReadinessForFirstGradeAsync(PreSchoolReadinessModel model)
        {
            if (model == null) throw new ApiException(Messages.EmptyModelError);

            var entity = await _context.PreSchoolReadinesses.SingleOrDefaultAsync(i => i.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                entity.Contents = model.Contents;
                await SaveAsync();
            }
        }

        public async Task Delete(int id)
        {
            PreSchoolEvaluation entity = await _context.PreSchoolEvaluations.SingleOrDefaultAsync(i => i.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                _context.PreSchoolEvaluations.Remove(entity);
                await SaveAsync();
            }
        }

        public async Task DeleteReadiness(int id)
        {
            PreSchoolReadiness entity = await _context.PreSchoolReadinesses.SingleOrDefaultAsync(i => i.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                _context.PreSchoolReadinesses.Remove(entity);
                await SaveAsync();
            }
        }
    }
}
