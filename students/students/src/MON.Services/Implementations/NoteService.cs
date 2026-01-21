namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models.Grid;
    using MON.Models.NoteModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MON.Shared;
    using System.Linq.Dynamic.Core;
    using MON.Models.Refugee;
    using System.Collections.Generic;

    public class NoteService : BaseService<NoteService>, INoteService
    {
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly IInstitutionService _institutionService;

        public NoteService(DbServiceDependencies<NoteService> dependencies,
            ILodFinalizationService lodFinalizationService,
            IInstitutionService institutionService)
            : base(dependencies)
        {
            _lodFinalizationService = lodFinalizationService;
            _institutionService = institutionService;
        }

        public async Task CreateNote(NoteModel noteModel)
        {
            if (!await _authorizationService.HasPermissionForStudent(noteModel?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentNoteManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var entry = new Note
            {
                PersonId = noteModel.PersonId,
                IssueDate = noteModel.IssueDate,
                Content = noteModel.Content,
                Title = noteModel.Title,
                SchoolYear = noteModel.SchoolYear ?? await _institutionService.GetCurrentYear(noteModel.InstitutionId ?? _userInfo?.InstitutionID),
                InstitutionId = noteModel.InstitutionId ?? _userInfo?.InstitutionID,
            };

            _context.Notes.Add(entry);
            await SaveAsync();
        }

        public async Task<NoteModel> GetById(int noteId)
        {
            NoteModel model = await _context.Notes
                .Where(n => n.Id == noteId)
                .Select(n => new NoteModel
                {
                    NoteId = noteId,
                    Content = n.Content,
                    IssueDate = n.IssueDate,
                    Title = n.Title,
                    SchoolYear = n.SchoolYear,
                    PersonId = n.PersonId,
                })
                .FirstOrDefaultAsync();

            if (model != null
              && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentNoteRead)
              && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentNoteManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<IPagedList<NoteViewModel>> List(NotesListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(NotesListInput)));
            }

            IQueryable<NoteViewModel> query = _context.VNotesLists
                .Where(x => x.PersonId == input.PersonId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Title.Contains(input.Filter)
                   || predicate.Content.Contains(input.Filter)
                   || predicate.IssueDate.ToString().Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter))
                .Select(x => new NoteViewModel
                {
                    NoteId = x.Id,
                    PersonId= x.PersonId,
                    Title = x.Title,
                    Content = x.Content,
                    IssueDate = x.IssueDate,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    InstitutionName = x.InstitutionName,
                    IsLodFinalized = x.LodIsFinalized
                }).OrderBy(string.IsNullOrEmpty(input.SortBy) ? "IssueDate desc" : input.SortBy);

            int totalCount = await query.CountAsync();

            IList<NoteViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task Update(NoteModel noteModel)
        {
            Note entity = await _context.Notes
              .SingleOrDefaultAsync(d => d.Id == noteModel.NoteId);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentNoteManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Title = noteModel.Title;
            entity.Content = noteModel.Content;
            entity.IssueDate = noteModel.IssueDate;
            entity.InstitutionId = noteModel.InstitutionId ?? _userInfo?.InstitutionID;
            entity.SchoolYear = noteModel.SchoolYear ?? await _institutionService.GetCurrentYear(noteModel.InstitutionId ?? _userInfo?.InstitutionID);

            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            Note entity = await _context.Notes
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentNoteManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            _context.Notes.Remove(entity);

            await SaveAsync();
        }
    }
}
