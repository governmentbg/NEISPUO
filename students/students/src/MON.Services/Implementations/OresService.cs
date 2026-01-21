namespace MON.Services.Implementations
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Grid;
    using MON.Models.Ores;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;
    using MON.Shared;
    using System.Linq.Dynamic.Core;
    using MON.Models.Dropdown;
    using System.Threading;

    public class OresService : BaseService<OresService>, IOresService
    {
        private readonly IInstitutionService _institutionService;
        private readonly IStudentService _studentService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public OresService(DbServiceDependencies<OresService> dependencies,
            IInstitutionService institutionService,
            IStudentService studentService,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _studentService = studentService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
        private async Task ProcessAddedDocs(OresModel model, Ore entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.OresAttachments.Add(new OresAttachment
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(OresModel model, Ore entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.OresAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        private async Task CheckPermission(string permission, int? personId, int? classId, int? institutionId)
        {
            if (personId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForStudent(personId.Value, permission))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
                else
                {
                    return;
                }
            }

            if (classId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForClass(classId.Value, permission))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
                else
                {
                    return;
                }
            }

            if (institutionId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForInstitution(institutionId.Value, permission))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
                else
                {
                    return;
                }
            }

            if (!await _authorizationService.AuthorizeUser(permission))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
            else
            {
                return;
            }

            throw new ApiException(Messages.UnauthorizedMessageError, 401);
        }

        private async Task ValidateModel(OresModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentException(nameof(model), nameof(OresModel)));
            }

            if (!model.EndDate.HasValue)
            {
                throw new ApiException("Полето \"От дата\" е задължително.");
            }

            if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }


            IQueryable<VOresList> query = _context.VOresLists;
            if (model.Id.HasValue)
            {
                // Редакция
                query = query.Where(x => x.OresId != model.Id.Value);
            }

            if (model.ClassId.HasValue)
            {
                query = query.Where(x => (x.OresEntityClassId.HasValue && x.OresEntityClassId == model.ClassId.Value)
                    || (x.OresEntityInstitutinId.HasValue && x.ClassId == model.ClassId.Value));

                VOresList existingOverlap = await query.FirstOrDefaultAsync(x => x.StartDate <= model.EndDate && model.StartDate <= x.EndDate);
                if (existingOverlap != null)
                {
                    throw new ApiException("Избраният период се припокрива с друг, в който класът или институцията са били в ОРЕС");
                }
            }

            if (model.InstitutionId.HasValue)
            {
                query = query.Where(x => x.OresEntityInstitutinId.HasValue && x.OresEntityInstitutinId == model.InstitutionId.Value);
                VOresList existingOverlap = await query.FirstOrDefaultAsync(x => x.StartDate <= model.EndDate && model.StartDate <= x.EndDate);
                if (existingOverlap != null)
                {
                    throw new ApiException("Избраният период се припокрива с друг, в който институцията е била в ОРЕС");
                }
            }

            if (model.PersonId.HasValue)
            {
                query = query.Where(x => x.PersonId == model.PersonId.Value);
                VOresList existingOverlap = await query.FirstOrDefaultAsync(x => x.StartDate <= model.EndDate && model.StartDate <= x.EndDate);
                if (existingOverlap != null)
                {
                    throw new ApiException("Избраният период се припокрива с друг, в който ученикът е бил в ОРЕС");
                }
            }

        }

        private IQueryable<VOresList> FilterByInput(IQueryable<VOresList> query, OresListInput input)
        {

            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(OresListInput)));
            }

            if (input.PersonId.HasValue)
            {
                query = query.Where(x => x.PersonId.HasValue && x.PersonId.Value == input.PersonId.Value);
            }

            if (input.ClassId.HasValue)
            {
                query = query.Where(x => x.ClassId.HasValue && x.ClassId.Value == input.ClassId.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId.HasValue && x.InstitutionId.Value == input.InstitutionId.Value);
            }

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId.HasValue && x.InstitutionId.Value == _userInfo.InstitutionID.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RegionId.HasValue && x.RegionId.Value == _userInfo.RegionID.Value);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.OresTypeId.HasValue)
            {
                query = query.Where(x => x.OresTypeId == input.OresTypeId.Value);
            }

            if (input.OresId.HasValue)
            {
                query = query.Where(x => x.OresId == input.OresId.Value);
            }

            if (!input.InheritanceType.IsNullOrEmpty())
            {
                query = input.InheritanceType switch
                {
                    "institution" => query.Where(x => x.IsInheritedFromInstitution == true),
                    "class" => query.Where(x => x.IsInheritedFromClass == true),
                    "student" => query.Where(x => x.IsInheritedFromInstitution != true && x.IsInheritedFromClass != true),
                    _ => query,
                };
            }

            if (input.StartDate.HasValue)
            {
                query = query.Where(x => x.StartDate == input.StartDate && x.EndDate == input.EndDate);
            }

            return query;
        }
        #endregion

        public async Task<OresViewModel> GetById(int id, CancellationToken  cancellationToken)
        {
            OresViewModel model = await _context.Ores
                .Where(x => x.Id == id)
                .Select(x => new OresViewModel
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    OresTypeId = x.OresTypeId,
                    OresTypeName = x.OresType.Name,
                    Description = x.Description,
                    Documents = x.OresAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    PersonId = x.OresToEntities.FirstOrDefault() != null ? x.OresToEntities.FirstOrDefault().PersonId : null,
                    ClassId = x.OresToEntities.FirstOrDefault() != null ? x.OresToEntities.FirstOrDefault().ClassId : null,
                    InstitutionId = x.OresToEntities.FirstOrDefault() != null ? x.OresToEntities.FirstOrDefault().InstitutionId : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentException(nameof(model), nameof(OresViewModel)));
            }

            await CheckPermission(DefaultPermissions.PermissionNameForOresRead, model.PersonId, model.ClassId, model.InstitutionId);

            return model;
        }

        public async Task<IPagedList<OresViewModel>> List(OresListInput input, CancellationToken cancellationToken)
        {
           await CheckPermission(DefaultPermissions.PermissionNameForOresRead, input?.PersonId, input?.ClassId, input?.InstitutionId);

            IQueryable<VOresList> query = FilterByInput(_context.VOresLists.AsNoTracking(), input);

            IQueryable<OresViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.OresTypeName.Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.OresEntityInstitutinId.ToString().Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.ClassName.Contains(input.Filter)
                   || predicate.BasicClassName.Contains(input.Filter)
                   || predicate.EduFormName.Contains(input.Filter)
                   || predicate.TownName.Contains(input.Filter)
                   || predicate.MunicipalityName.Contains(input.Filter)
                   || predicate.RegionName.Contains(input.Filter))
                .Select(x => new OresViewModel
                {
                    Id = x.OresId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    OresTypeId = x.OresTypeId,
                    OresTypeName = x.OresTypeName,
                    Description = x.Description,
                    InstitutionId = x.OresEntityInstitutinId ?? x.InstitutionId,
                    ClassId = x.OresEntityClassId,
                    PersonId = x.OresEntityPersonId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    FullName = x.FullName,
                    Pin = x.Pin,
                    PinTypeName = x.PinTypeName,
                    IsInheritedFromClass = x.IsInheritedFromClass ?? false,
                    IsInheritedFromInstitution = x.IsInheritedFromInstitution ?? false,
                    EnrollmentDate = x.EnrollmentDate,
                    DischargeDate = x.DischargeDate,
                    InstitutionName = x.InstitutionName,
                    ClassName = x.ClassName,
                    BasicClassName = x.BasicClassRomeName,
                    EduFormName = x.EduFormName,
                    TownName = x.TownName,
                    MunicipalityName = x.MunicipalityName,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    PersonOresStartDate = x.PersonOresStartDate,
                    PersonOresEndDate = x.PersonOresEndDate,
                    PersonOresCalendarDaysCount = x.PersonOresCalendarDaysCount,
                    PersonOresWorkDaysCount = x.PersonOresWorkDaysCount,
                    HasManagePermission = x.CreatorInstitutionId != null && x.CreatorInstitutionId == _userInfo.InstitutionID,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "StartDate desc, FullName asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            IList<OresViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.Uid = Guid.NewGuid().ToString(); // Използва се за ключ на грида
            }

            return items.ToPagedList(totalCount);
        }

        public async Task Create(OresModel model)
        {
            await ValidateModel(model);
            await CheckPermission(DefaultPermissions.PermissionNameForOresManage, model.PersonId, model.ClassId, model.InstitutionId);

            Ore entry = new Ore
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                OresTypeId = model.OresTypeId,
                Description = model.Description,
                InstitutionId = _userInfo.InstitutionID
            };

            short currentSchoolYear = await _institutionService.GetCurrentYear(model.InstitutionId);

            if (model.PersonId.HasValue)
            {
                entry.OresToEntities.Add(new OresToEntity
                {
                    PersonId = model.PersonId,
                    SchoolYear = currentSchoolYear,
                    StudentClassId = (await _studentService.GetMainStudentClass(model.PersonId.Value, true, currentSchoolYear,_userInfo.InstitutionID))?.Id
                });
            }

            if (model.ClassId.HasValue)
            {
                entry.OresToEntities.Add(new OresToEntity
                {
                    ClassId = model.ClassId,
                    SchoolYear = currentSchoolYear
                });
            }

            if (model.InstitutionId.HasValue)
            {
                entry.OresToEntities.Add(new OresToEntity
                {
                    InstitutionId = model.InstitutionId,
                    SchoolYear = currentSchoolYear
                });
            }

            using var transaction = _context.Database.BeginTransaction();
            _context.Ores.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(OresModel model)
        {
            Ore entity = await _context.Ores
                .Include(x => x.OresAttachments)
                .Include(x => x.OresToEntities)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(entity), nameof(Ore)));
            }

            await ValidateModel(model);

            // Само институцията създател на ОРЕС записа има право да я редактира;
            await CheckPermission(DefaultPermissions.PermissionNameForOresManage, null, null, entity.InstitutionId);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            using var transaction = _context.Database.BeginTransaction();

            entity.OresTypeId = model.OresTypeId;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.Description = model.Description;


            if (entity.OresToEntities.Any(x => x.PersonId.HasValue && !x.StudentClassId.HasValue))
            {
                foreach (var item in entity.OresToEntities.Where(x => x.PersonId.HasValue && !x.StudentClassId.HasValue))
                {
                    item.StudentClassId = (await _studentService.GetMainStudentClass(model.PersonId.Value, true, item.SchoolYear, _userInfo.InstitutionID))?.Id;
                }
            }

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();

        }

        public async Task Delete(int id)
        {
            Ore entity = await _context.Ores
                .Include(x => x.OresToEntities)
                .Include(x => x.OresAttachments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(entity), nameof(Ore)));
            }

            // Само институцията създател на ОРЕС записа има право да я изтрива;

            await CheckPermission(DefaultPermissions.PermissionNameForOresManage, null, null, entity.InstitutionId);

            if (!entity.OresAttachments.IsNullOrEmpty())
            {
                var docsIds = entity.OresAttachments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.OresAttachments.RemoveRange(entity.OresAttachments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            if (!entity.OresToEntities.IsNullOrEmpty())
            {
                _context.OresToEntities.RemoveRange(entity.OresToEntities);
            }

            _context.Ores.Remove(entity);

            await SaveAsync();
        }

        public async Task<OresDetailsViewModel[]> GetCalendarDetails(DateTime start, DateTime end, int? regionId, int? institutionId, CancellationToken cancellationToken)
        {
            await CheckPermission(DefaultPermissions.PermissionNameForOresRead, null, null, null);

            IQueryable<VOresCalendarDetail> query = _context.VOresCalendarDetails
                .AsNoTracking()
                .Where(x => x.StartDate <= end.AddDays(1) && start <= x.EndDate);

            if (regionId.HasValue)
            {
                query = query.Where(x => x.RegionId == regionId.Value);
            }

            if (institutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == institutionId.Value);
            }

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RegionId == _userInfo.RegionID.Value);
            }

            OresDetailsViewModel[] list = await query.OrderBy(x => x.StartDate)
                .Select(x => new OresDetailsViewModel
                {
                    Id = x.OresId,
                    StartDate = x.StartDate ?? default,
                    EndDate = x.EndDate,
                    OresTypeId = x.OresTypeId ?? default,
                    OresTypeName = x.OresTypeName,
                    SchoolYear = x.SchoolYear ?? default,
                    InstitutionId = x.InstitutionId ?? default,
                    InstitutionName = x.InstitutionName,
                    RegionId = x.RegionId ?? default,
                    RegionName = x.RegionName,
                    StudentsCount = x.StudentsCount ?? 0,
                    Description = x.Description,
                    IsInheritedFromInstitution = x.IsInheritedFromInstitution,
                    IsInheritedFromClass = x.IsInheritedFromClass,
                    StudentFullName = x.StudentFullName,
                    ClassName = x.ClassName
                })
                .ToArrayAsync(cancellationToken);

            foreach (var item in list)
            {
                item.CalendarEventTitle = item.GetCalendarEventTitle(_userInfo.InstitutionID);
            }

            return list;
                
        }

        public async Task<OresRangeDropdownViewModel[]> GetOresRangeDropdownOptions([FromQuery] OresListInput input, CancellationToken cancellationToken)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input), nameof(OresListInput));


            // Задължително трябва да има филтър по учебна година с цел бързодействие.
            if (!input.SchoolYear.HasValue)
            {
                input.SchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID ?? input.InstitutionId);
            }

            IQueryable<VOresList> query = FilterByInput(_context.VOresLists.AsNoTracking(), input);

            OresRangeDropdownViewModel[] models = await query.GroupBy(x => new { x.StartDate, x.EndDate })
                .Select(x => new OresRangeDropdownViewModel
                {
                    StartDate = x.Key.StartDate,
                    EndDate = x.Key.EndDate,
                })
                .OrderByDescending(x => x.StartDate)
                .ThenBy(x => x.EndDate)
                .ToArrayAsync(cancellationToken);

            return models.Select((x, index) =>
            {
                x.Text = $"{x.StartDate:dd/MM/yyyy} - {x.EndDate:dd/MM/yyyy}";
                x.Value = index;

                return x;
            }).ToArray();
        }

    }
}
