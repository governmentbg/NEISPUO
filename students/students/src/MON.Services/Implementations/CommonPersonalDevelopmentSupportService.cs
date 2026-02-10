namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Options;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.Models.Ores;
    using MON.Shared;
    using System;
    using MON.Services.Security.Permissions;
    using System.Collections.Generic;
    using System.Linq.Dynamic.Core;
    using MON.Shared.ErrorHandling;
    using MON.DataAccess;
    using Z.EntityFramework.Plus;

    public class CommonPersonalDevelopmentSupportService : BaseService<CommonPersonalDevelopmentSupportService>, ICommonPersonalDevelopmentSupportService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public CommonPersonalDevelopmentSupportService(DbServiceDependencies<CommonPersonalDevelopmentSupportService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _blobServiceConfig = blobServiceConfig.Value;
            _blobService = blobService;
        }

        #region Private members

        private async Task CheckPermission(string permission, int? personId)
        {
            if(!personId.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if(!await _authorizationService.HasPermissionForStudent(personId.Value, permission))
                {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        private async Task ValidateModel(CommonPersonalDevelopmentSupportModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentException(nameof(model), nameof(CommonPersonalDevelopmentSupportModel)));
            }

            if (await _context.CommonPersonalDevelopmentSupports.AnyAsync(x => x.Id != model.Id
                && x.PersonId == model.PersonId
                && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException(Messages.InvalidOperation,
                    new InvalidOperationException($"Обща подкрепа за личностно развитие за този ученик и учебна година {model.SchoolYear} вече съществува!"));
            }
        }

        private async Task ProcessAddedDocs(CommonPersonalDevelopmentSupportModel model, CommonPersonalDevelopmentSupport entity)
        {
            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.CommonPersonalDevelopmentSupportAttachments.Add(new CommonPersonalDevelopmentSupportAttachment
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(CommonPersonalDevelopmentSupportModel model)
        {
            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            if (docIdsToDelete.Count > 0)
            {
                await _context.CommonPersonalDevelopmentSupportAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
                await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
            }

        }
        #endregion

        public async Task<CommonPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            CommonPersonalDevelopmentSupportViewModel model = await _context.CommonPersonalDevelopmentSupports
                .Where(x => x.Id == id)
                .Select(x => new CommonPersonalDevelopmentSupportViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    Items = x.CommonPersonalDevelopmentSupportItems
                        .Select(i => new CommonPersonalDevelopmentSupportItemViewModel
                        {
                            Id = i.Id,
                            TypeId = i.TypeId,
                            TypeName = i.Type.Name,
                            Details = i.Details
                        }),
                    Documents = x.CommonPersonalDevelopmentSupportAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentException(nameof(model), nameof(OresViewModel)));
            }

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead, model.PersonId);

            return model;
        }

        public async Task<IPagedList<CommonPersonalDevelopmentSupportViewModel>> List(StudentListInput input, CancellationToken cancellationToken)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead, input.StudentId);

            IQueryable<CommonPersonalDevelopmentSupportViewModel> query = _context.CommonPersonalDevelopmentSupports
                .Where(x => x.PersonId == input.StudentId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.SchoolYearNavigation.Name.Contains(input.Filter)
                    || predicate.CommonPersonalDevelopmentSupportItems.Any(x => x.Type.Name.Contains(input.Filter)))
                .Select(x => new CommonPersonalDevelopmentSupportViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    Items = x.CommonPersonalDevelopmentSupportItems
                        .Select(i => new CommonPersonalDevelopmentSupportItemViewModel
                        {
                            Id = i.Id,
                            TypeId = i.TypeId,
                            TypeName = i.Type.Name,
                        }),
                    Documents = x.CommonPersonalDevelopmentSupportAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Id desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            IList<CommonPersonalDevelopmentSupportViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task Create(CommonPersonalDevelopmentSupportModel model)
        {
            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, model.PersonId);
            await ValidateModel(model);

            CommonPersonalDevelopmentSupport entry = new CommonPersonalDevelopmentSupport
            {
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear,
                CommonPersonalDevelopmentSupportItems = model.Items.Select(x => new CommonPersonalDevelopmentSupportItem
                {
                    TypeId = x.TypeId,
                    Details = x.Details,
                }).ToList(),
            };

            using var transaction = _context.Database.BeginTransaction();
            _context.CommonPersonalDevelopmentSupports.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(CommonPersonalDevelopmentSupportModel model)
        {
            CommonPersonalDevelopmentSupport entity = await _context.CommonPersonalDevelopmentSupports
                .Include(x => x.CommonPersonalDevelopmentSupportItems)
                .Include(x => x.CommonPersonalDevelopmentSupportAttachments)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, entity?.PersonId);
            await ValidateModel(model);

            using var transaction = _context.Database.BeginTransaction();

            entity.SchoolYear = model.SchoolYear;

            HashSet<int> existedIds = model.Items.Where(x => x.Id.HasValue && x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet();

            var toDelete = entity.CommonPersonalDevelopmentSupportItems.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                _context.CommonPersonalDevelopmentSupportItems.RemoveRange(toDelete);
            }

            var toAdd = model.Items.Where(x => !(x.Id.HasValue && x.Id.Value != 0));
            foreach (var item in toAdd)
            {
                entity.CommonPersonalDevelopmentSupportItems.Add(new CommonPersonalDevelopmentSupportItem
                {
                    TypeId = item.TypeId,
                    Details = item.Details,
                });
            }

            foreach (var toUpdate in entity.CommonPersonalDevelopmentSupportItems.Where(x => existedIds.Contains(x.Id)))
            {
                var source = model.Items.SingleOrDefault(x => x.Id.HasValue && x.Id.Value != 0 && x.Id.Value == toUpdate.Id);
                if (source == null) continue;

                toUpdate.TypeId = source.TypeId;
                toUpdate.Details = source.Details;
            }

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            CommonPersonalDevelopmentSupport entity = await _context.CommonPersonalDevelopmentSupports
                .Include(x => x.CommonPersonalDevelopmentSupportItems)
                .Include(x => x.CommonPersonalDevelopmentSupportAttachments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(entity), nameof(Ore)));
            }

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, entity.PersonId);

            if (!entity.CommonPersonalDevelopmentSupportAttachments.IsNullOrEmpty())
            {
                _context.CommonPersonalDevelopmentSupportAttachments.RemoveRange(entity.CommonPersonalDevelopmentSupportAttachments);
            }

            if (!entity.CommonPersonalDevelopmentSupportItems.IsNullOrEmpty())
            {
                _context.CommonPersonalDevelopmentSupportItems.RemoveRange(entity.CommonPersonalDevelopmentSupportItems);
            }

            _context.CommonPersonalDevelopmentSupports.Remove(entity);

            await SaveAsync();
        }
    }
}
