using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models.ExternalEvaluation;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class ExternalEvaluationService : BaseService<ExternalEvaluationService>, IExternalEvaluationService
    {
        private readonly ILodFinalizationService _lodFinalizationService;

        public ExternalEvaluationService(DbServiceDependencies<ExternalEvaluationService> dependencies,
            ILodFinalizationService lodFinalizationService)
            : base(dependencies)
        {
            _lodFinalizationService = lodFinalizationService;
        }

        public async Task<List<ExternalEvaluationModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentExternalEvaluationRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.ExternalEvaluations
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new ExternalEvaluationModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    TypeId = x.ExternalEvaluationTypeId,
                    Type = x.ExternalEvaluationType.Name,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    Uid = x.Id.ToString(),
                    Description = x.Description,
                    Evaluations = x.ExternalEvaluationItems.Select(e => new ExternalEvaluationItemModel
                    {
                        Id = e.Id,
                        Subject = e.Subject,
                        OriginalPoints = e.OriginalPoints,
                        Points = e.Points,
                        Grade = e.Grade,
                        Description = e.Description,
                        FLLevel = e.Fllevel,
                        SubjectId = e.SubjectId,
                        SubjectTypeId = e.SubjectTypeId,
                        SubjectTypeName = e.SubjectType.Name
                    }).OrderBy(x => x.Subject).ToList()
                })
                .OrderBy(x => x.TypeId)
                .ThenBy(x => x.ParentId)
                .ToListAsync();
        }

        public async Task Create(ExternalEvaluationModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentExternalEvaluationManage)
                 && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentExternalEvaluationManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model.ParentId.HasValue && await _context.ExternalEvaluations.AnyAsync(x => x.Id == model.ParentId.Value && x.ParentId.HasValue))
            {
                throw new ArgumentException("Не може да се създаде поправка на поправката.");
            }

            if (model.SchoolYear.HasValue)
            {
                if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear.Value))
                {
                    throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
                }
            }

            ExternalEvaluation entry = new ExternalEvaluation
            {
                ParentId = model.ParentId,
                PersonId = model.PersonId,
                ExternalEvaluationTypeId = model.TypeId,
                SchoolYear = model.SchoolYear,
                Description = model.Description,
                ExternalEvaluationItems = model.Evaluations?.Select(x => new ExternalEvaluationItem
                {
                    SubjectId = x.SubjectId.Value,
                    Subject = _context.Subjects.FirstOrDefault(i => i.SubjectId == x.SubjectId)?.SubjectName,
                    SubjectTypeId = x.SubjectTypeId,
                    Points = x.Points ?? default,
                    OriginalPoints = x.OriginalPoints ?? default,
                    Grade = x.Grade
                }).ToList()
            };

            _context.ExternalEvaluations.Add(entry);

            await SaveAsync();
        }

        public async Task Update(ExternalEvaluationModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            ExternalEvaluation entity = await _context.ExternalEvaluations
                .Include(x => x.ExternalEvaluationItems)
                .Where(x => x.Id == model.Id)
                .SingleOrDefaultAsync();

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentExternalEvaluationManage)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentExternalEvaluationManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model.SchoolYear.HasValue)
            {
                if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear.Value))
                {
                    throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
                }
            }

            entity.SchoolYear = model.SchoolYear;
            entity.Description = model.Description;

            HashSet<int> existedIds = model.Evaluations != null
                ? model.Evaluations.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.ExternalEvaluationItems.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Оценки за изтриване
                _context.ExternalEvaluationItems.RemoveRange(toDelete);
            }

            if (model.Evaluations != null)
            {
                // Оценки за добавяне
                var toAdd = model.Evaluations.Where(x => !x.Id.HasValue);
                if (toAdd.Any())
                {
                    _context.ExternalEvaluationItems.AddRange(toAdd.Select(x => new ExternalEvaluationItem
                    {
                        ExternalEvaluationId = entity.Id,
                        Subject = _context.Subjects.FirstOrDefault(i => i.SubjectId == x.SubjectId)?.SubjectName,
                        SubjectId = x.SubjectId.Value,
                        SubjectTypeId = x.SubjectTypeId.Value,
                        OriginalPoints = x.OriginalPoints ?? default,
                        Points = x.Points ?? default,
                        Grade = x.Grade
                    }));

                }
            }

            if (existedIds.Any())
            {
                // Оценки за редакция
                foreach (var toUpdate in entity.ExternalEvaluationItems.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.Evaluations.Where(x => x.Id.HasValue && x.Id.Value == toUpdate.Id).SingleOrDefault();
                    if (source == null) continue;

                    toUpdate.Subject = source.Subject;
                    toUpdate.SubjectId = source.SubjectId.Value;
                    toUpdate.Subject = _context.Subjects.FirstOrDefault(i => i.SubjectId == source.SubjectId)?.SubjectName;
                    toUpdate.SubjectTypeId = source.SubjectTypeId;
                    toUpdate.Points = source.Points ?? default;
                    toUpdate.OriginalPoints = source.OriginalPoints ?? default;
                    toUpdate.Grade = source.Grade;
                }
            }

            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            ExternalEvaluation entity = await _context.ExternalEvaluations
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentExternalEvaluationManage)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentExternalEvaluationManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }
            if (entity.SchoolYear.HasValue)
            {
                if (await _lodFinalizationService.IsLodFInalized(entity.PersonId, entity.SchoolYear.Value))
                {
                    throw new InvalidOperationException(Messages.LodIsFinalizedError(entity?.SchoolYear));
                }
            }


            if (entity != null)
            {
                _context.ExternalEvaluations.Remove(entity);
                await SaveAsync();
            }
        }
    }
}
