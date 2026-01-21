namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Issue;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Enums;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using Helpdesk.Services.Extensions;
    using System.Collections.Generic;
    using Helpdesk.Shared;
    using Microsoft.Extensions.Options;
    using Z.EntityFramework.Plus;
    using Helpdesk.Models.Configuration;
    using System.Security.Cryptography;
    using Org.BouncyCastle.Bcpg.OpenPgp;

    public class IssueService : BaseService, IIssueService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IEmailService _emailService;
        private readonly IMessageService _messageService;
        private readonly IAppConfigurationService _configurationService;
        private const string Level2NotificationEmailKey = "Level2NotificationEmail";
        private const string Level3NotificationEmailKey = "Level3NotificationEmail";
        // Речник с e-mail-и, на които да се изпрати съобщение, тъй като нямаме достъп до edu.mon.bg
        Dictionary<string, string> ConsortiumEmails = new Dictionary<string, string> { { "neispuo_kontrax@edu.mon.bg", "neispuo_support@kontrax.bg" } };

        public IssueService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<IssueService> logger,
            IBlobService blobService,
            IEmailService emailService,
            IMessageService messageService,
            IAppConfigurationService configurationService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(context, userInfo, logger)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _emailService = emailService;
            _messageService = messageService;
            _configurationService = configurationService;
        }


        #region Private members
        /// <summary>
        /// Текуща учебна година за институцията
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        private async Task<short> GetCurrentYear(int? institutionId)
        {
            return institutionId.HasValue
                ? await CurrentSchoolYearForInstitution(institutionId.Value)
                : await CurrentSchoolYear();
        }

        private async Task<short> CurrentSchoolYearForInstitution(int institutionId)
        {
            return await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == institutionId && x.IsCurrent == true)
                .Select(x => x.SchoolYear)
                .FirstOrDefaultAsync();
        }

        private async Task<short> CurrentSchoolYear()
        {
            return await _context.CurrentYears
                .Where(x => x.IsValid == true)
                .Select(x => x.CurrentYearId)
                .FirstOrDefaultAsync();
        }
        private async Task ProcessAddedDocs(IssueModel model, Issue entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                if (!result.IsError)
                {
                    entity.IssueAttachments.Add(new IssueAttachment
                    {
                        Document = docModel.ToDocument(result.Data?.BlobId)
                    });
                }
                else
                {
                    entity.Description += Environment.NewLine + $"[{result.Message}]";
                }
            }

            if (model.Documents.Any(x => x.HasToAdd()))
            {
                entity.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = entity.StatusId,
                    Comment = Messages.AddFiles,
                    IsHidden = true
                });
            }
        }

        private async Task ProcessDeletedDocs(IssueModel model, Issue entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            if (docIdsToDelete.Count > 0)
            {
                await _context.IssueAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
                await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
                entry.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = entry.StatusId,
                    Comment = Messages.DeleteFiles,
                    IsHidden = true
                });
            }
        }

        private async Task ProcessAddedDocs(IssueCommentModel model, IssueComment entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                if (!result.IsError)
                {
                    entity.IssueCommentAttachments.Add(new IssueCommentAttachment
                    {
                        Document = docModel.ToDocument(result.Data?.BlobId)
                    });
                }
                else
                {
                    entity.Comment += Environment.NewLine + $"[{result.Message}]";
                }
            }
        }
        private async Task<string> GetNotificationEmailForCategory(int categoryId)
        {
            var notificationEmail = await (
                from c in _context.Categories
                where c.Id == categoryId
                select c.ParentId != null ? c.Parent.NotificationEmail : c.NotificationEmail).FirstOrDefaultAsync();

            if (String.IsNullOrWhiteSpace(notificationEmail))
            {
                notificationEmail = await _configurationService.GetValueByKey(Level3NotificationEmailKey);
            }

            return notificationEmail;
        }

        private async Task<List<int>> GetAllUsersForIssue(int id)
        {
            var commentUsers = await (
                from comment in _context.IssueComments
                where comment.IssueId == id
                select comment.CreatedBySysUserId).ToListAsync();

            var issue = _context.Issues.Find(id);

            commentUsers.Add(issue.SubmitterSysUserId);

            return commentUsers.Distinct().ToList();
        }

        private async Task<List<string>> GetEmailsForUsers(List<int> userIds)
        {
            var emails = await (
                from s in _context.SysUsers
                where userIds.Contains(s.SysUserId)
                select s.Username).ToListAsync();

            var additionalEmails = (from e in emails
                    from d in ConsortiumEmails
                    where e == d.Key
                    select d.Value).ToList();
            if (additionalEmails.Any()) {
                _logger.LogWarning($"Допълнителни e-mail-и за {string.Join(",", userIds)}: {string.Join(",", additionalEmails)}");
                emails.AddRange(additionalEmails);
            }
            return emails;
        }

        private async Task<string> GetEmailForUser(int userId)
        {
            var email = await (
                from s in _context.SysUsers
                where s.SysUserId == userId
                select s.Username).FirstOrDefaultAsync();

            return email;
        }

        private async Task<string> GetUsernameForUser(int? userId)
        {
            if (!userId.HasValue) return "";

            return await _context.SysUsers
                .Where(x => x.SysUserId == userId.Value)
                .Select(x => x.Username)
                .FirstOrDefaultAsync();
        }
        #endregion

        public async Task<IPagedList<IssueViewModel>> GetListAsync(IssuePageListInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(PagedListInput));

            IQueryable<Issue> query = _context.Issues.AsNoTracking();

            if (input.Status.HasValue)
            {
                query = query.Where(x => x.StatusId == input.Status.Value);
            }

            if (input.Category.HasValue)
            {
                query = query.Where(x => x.CategoryId == input.Category.Value);
            }

            if (input.Priority.HasValue)
            {
                query = query.Where(x => x.PriorityId == input.Priority.Value);
            }

            if (input.IsEscalated.HasValue)
            {
                query = query.Where(x => x.IsEscalated == input.IsEscalated.Value);
            }

            //if (input.AssignedToMe == true)
            //{
            //    query = query.Where(x => x.AssignedToSysUserId != null && x.AssignedToSysUserId == _userInfo.SysUserID);
            //}

            switch (input.AssignLevel)
            {
                case (int)AssignLevelEnum.AssignedToMe:
                    query = query.Where(x => x.AssignedToSysUserId != null && x.AssignedToSysUserId == _userInfo.SysUserID);
                    break;
                case (int)AssignLevelEnum.NotAssignedToMe:
                    query = query.Where(x => x.AssignedToSysUserId != null && x.AssignedToSysUserId != _userInfo.SysUserID);
                    break;
                case (int)AssignLevelEnum.Unassigned:
                    query = query.Where(x => x.AssignedToSysUserId == null);
                    break;
            }

            if (input.SupportLevel.HasValue)
            {
                switch (input.SupportLevel)
                {
                    case 0:
                        query = query.Where(x => !x.IsEscalated && !x.IsLevel3Support);
                        break;
                    case 1:
                        query = query.Where(x => x.IsEscalated && !x.IsLevel3Support);
                        break;
                    case 2:
                        query = query.Where(x => x.IsLevel3Support);
                        break;
                }
            }

            if (input.RequestForInformation.HasValue && input.RequestForInformation.Value)
            {
                query = query.Where(x => x.RequestForInformation == input.RequestForInformation);
            }

            switch (_userInfo.SysRoleID)
            {
                case (int)UserRoleEnum.School:
                    query = query.Where(i => i.SubmitterSysUserId == _userInfo.SysUserID);
                    break;
                case (int)UserRoleEnum.Municipality:
                    query = query.Where(i => i.SubmitterSysUserId == _userInfo.SysUserID);
                    break;
                case (int)UserRoleEnum.Ruo:
                case (int)UserRoleEnum.RuoExpert:
                    query = query.Where(i => i.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID || i.SubmitterSysUserId == _userInfo.SysUserID);
                    break;
                case (int)UserRoleEnum.Mon:
                case (int)UserRoleEnum.MonExpert:
                case (int)UserRoleEnum.Consortium:
                case (int)UserRoleEnum.CIOO:
                    break;
                default:
                    query = query.Where(i => false);
                    break;
            }

            int intFilter = 0;
            bool isNumber = Int32.TryParse(input.Filter, out intFilter);

            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Title.Contains(input.Filter)
                    || predicate.SubmitterSysUser.Username.Contains(input.Filter)
                    || predicate.Category.Name.Contains(input.Filter)
                    || predicate.Priority.Name.Contains(input.Filter)
                    || predicate.Status.Name.Contains(input.Filter)
                    || (isNumber && predicate.Id == intFilter)
                    || (input.SearchEverywhere.HasValue && input.SearchEverywhere.Value &&
                            (predicate.Description.Contains(input.Filter) || predicate.IssueComments.Any(i => i.Comment.Contains(input.Filter)))
                    ));

            IQueryable<IssueViewModel> issues = query
                .Select(i => new IssueViewModel()
                {
                    Id = i.Id,
                    Title = i.Title,
                    Phone = i.Phone,
                    Description = i.Description,
                    CategoryId = i.CategoryId,
                    Category = i.Category.Name,
                    Priority = i.Priority.Name,
                    Status = i.Status.Name,
                    PriorityId = i.PriorityId,
                    IsEscalated = i.IsEscalated,
                    IsLevel3Support = i.IsLevel3Support,
                    CreateDate = i.CreateDate,
                    ResolveDate = i.ResolveDate,
                    StatusId = i.StatusId,
                    SubcategoryId = i.SubcategoryId,
                    Subcategory = i.Subcategory.Name,
                    SubmitterUsername = i.SubmitterSysUser.Username,
                    SubmitterSysUserId = i.SubmitterSysUserId,
                    AssignedToSysUserId = i.AssignedToSysUserId,
                    AssignedToSysUser = i.AssignedToSysUser.Username,
                    RequestForInformation = i.RequestForInformation,
                    CommentsCount = i.IssueComments.Count(),
                    AttachmentsCount = i.IssueAttachments.Count() + i.IssueComments.SelectMany(x => x.IssueCommentAttachments).Count(),
                    Institution = i.InstitutionId == null ? null : new InstitutionModel()
                    {
                        InstitutionId = i.InstitutionSchoolYear.InstitutionId,
                        SchoolYear = i.InstitutionSchoolYear.SchoolYear,
                        InstitutionName = i.InstitutionSchoolYear.Name,
                        TownId = i.InstitutionSchoolYear.TownId,
                        MunicipalityId = i.InstitutionSchoolYear.Town.MunicipalityId,
                        RegionId = i.InstitutionSchoolYear.Town.Municipality.RegionId,
                    },
                    Documents = i.IssueAttachments
                        .Select(x => x.Document.ToViewModel(_blobServiceConfig)),
                    LastActivityDate = i.IssueStatusHistories.OrderByDescending(a => a.CreateDate).Select(a => a.CreateDate).FirstOrDefault(),
                    HasUnreadChanges = i.IssueStatusHistories
                        .Any(h => h.CreatedBySysUserId != _userInfo.SysUserID && h.Comment != Messages.Resolved && h.Comment != Messages.NewIssue && h.CreateDate >
                            i.IssueReadActivities
                                .Where(a => a.CreatedBySysUserId == _userInfo.SysUserID)
                                .OrderByDescending(a => a.Id).Select(a => a.LastActivityDate)
                                .FirstOrDefault())
                })
                 .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await issues.CountAsync();
            List<IssueViewModel> items = await issues.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        private async Task<IssueViewModel> GetViewModelById(int id)
        {
            var issue = await (
                from i in _context.Issues
                where i.Id == id
                select new IssueViewModel()
                {
                    Id = i.Id,
                    CategoryId = i.CategoryId,
                    Category = i.Category.Name,
                    HtmlDescription = i.Description.MakeHtml(true, ""),
                    Description = i.Description,
                    Title = i.Title,
                    Survey = i.Survey,
                    Phone = i.Phone,
                    IsEscalated = i.IsEscalated,
                    IsLevel3Support = i.IsLevel3Support,
                    StatusId = i.StatusId,
                    Status = i.Status.Name,
                    PriorityId = i.PriorityId,
                    Priority = i.Priority.Name,
                    SubcategoryId = i.SubcategoryId,
                    Subcategory = i.Subcategory.Name,
                    CreateDate = i.CreateDate,
                    ModifyDate = i.ModifyDate,
                    ResolveDate = i.ResolveDate,
                    SubmitterUsername = i.SubmitterSysUser.Username,
                    AssignedToSysUserId = i.AssignedToSysUserId,
                    AssignedToSysUser = i.AssignedToSysUser.Username,
                    RequestForInformation = i.RequestForInformation,
                    Documents = i.IssueAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Institution = i.InstitutionId == null ? null : new InstitutionModel()
                    {
                        InstitutionId = i.InstitutionSchoolYear.InstitutionId,
                        SchoolYear = i.InstitutionSchoolYear.SchoolYear,
                        InstitutionName = i.InstitutionSchoolYear.Name,
                        TownId = i.InstitutionSchoolYear.TownId,
                        Town = i.InstitutionSchoolYear.Town.Name,
                        MunicipalityId = i.InstitutionSchoolYear.Town.MunicipalityId,
                        Municipality = i.InstitutionSchoolYear.Town.Municipality.Name,
                        RegionId = i.InstitutionSchoolYear.Town.Municipality.RegionId,
                        Region = i.InstitutionSchoolYear.Town.Municipality.Region.Name,
                    },
                    StatusHistory = i.IssueStatusHistories
                        .Where(x => !x.IsHidden)
                        .OrderByDescending(x => x.CreateDate)
                        .Select(x => new IssueStatusHistoryViewModel
                        {
                            Comment = x.Comment,
                            CreateDate = x.CreateDate,
                            CreatorUsername = x.CreatedBySysUser.Username,
                        }),
                    Comments = i.IssueComments
                        .OrderByDescending(x => x.CreateDate)
                        .Select(x => new IssueCommentViewModel
                        {
                            IssueId = x.IssueId,
                            Comment = x.Comment.MakeHtml(true, ""),
                            CreateDate = x.CreateDate,
                            CreatorUsername = x.CreatedBySysUser.Username,
                            ModifyDate = x.ModifyDate,
                            ModifierUsername = x.ModifiedBySysUser.Username,
                            IsResolvingComment = x.IsResolvingComment,
                            Documents = x.IssueCommentAttachments
                                .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                        })
                }).FirstOrDefaultAsync();

            if (issue.Institution != null && issue.Institution.TownId != null)
            {
                issue.Institution.Address = $"гр./с.{issue.Institution.Town} общ.{issue.Institution.Municipality} обл.{issue.Institution.Region}";

                issue.Institution.Departments = await (
                    from department in _context.InstitutionDepartments
                    where department.InstitutionId == issue.Institution.InstitutionId
                    select new Department()
                    {
                        DepartmentId = department.InstitutionDepartmentId,
                        Name = department.Name,
                        Address = department.Address,
                        TownId = department.TownId,
                        Town = department.Town.Name
                    }).ToListAsync();
            }

            return issue;
        }

        public async Task<IssueViewModel> GetById(int id)
        {
            return await GetViewModelById(id);
        }

        public async Task<int> Create(IssueModel model)
        {
            var issue = new Issue()
            {
                Title = model.Title,
                Phone = model.Phone,
                Description = model.Description,
                IsEscalated = model.IsEscalated,
                IsLevel3Support = model.IsLevel3Support,
                AssignedToSysUserId = model.AssignedToSysUserId,
                CategoryId = model.CategoryId,
                PriorityId = model.PriorityId,
                SubcategoryId = model.SubcategoryId,
                SubmitterSysUserId = _userInfo.SysUserID,
                SubmitterSysRoleId = _userInfo.SysRoleID,
                InstitutionId = _userInfo.InstitutionID,
                SchoolYear = await GetCurrentYear(_userInfo.InstitutionID),
                Survey = model.Survey,
                StatusId = model.StatusId ?? 1 // 1-Нов
            };

            var category = await _context.Categories.FirstOrDefaultAsync(i => i.Id == model.CategoryId);
            var subCategory = await _context.Categories.FirstOrDefaultAsync(i => i.Id == model.SubcategoryId);

            if (issue.IsLevel3Support == false)
            {
                if (subCategory != null && subCategory.AutoElevate)
                {
                    issue.IsLevel3Support = true;
                }
                else if (category.AutoElevate)
                {
                    issue.IsLevel3Support = true;
                }
            }

            if (issue.AssignedToSysUserId == null)
            {
                if (subCategory != null && subCategory.AutoAssignToSysUserId != null)
                {
                    issue.AssignedToSysUserId = subCategory.AutoAssignToSysUserId;
                }
                else if (category.AutoAssignToSysUserId != null)
                {
                    issue.AssignedToSysUserId = category.AutoAssignToSysUserId;
                }
            }

            issue.IssueStatusHistories.Add(new IssueStatusHistory()
            {
                StatusId = (int)StatusEnum.New,
                Comment = Messages.NewIssue
            });

            if (issue.AssignedToSysUserId != null)
            {
                issue.StatusId = (int)StatusEnum.Assigned;
                issue.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = (int)StatusEnum.Assigned,
                    Comment = string.Format(Messages.AssignToSysUser, await GetUsernameForUser(issue.AssignedToSysUserId))
                });
                var user = await _context.SysUsers
                    .Where(x => x.SysUserId == issue.AssignedToSysUserId)
                    .Select(x => new { x.SysUserId, x.Username })
                    .FirstOrDefaultAsync();

                if (user.SysUserId != issue.SubmitterSysUserId)
                {
                    await _emailService.SendEmailAsync(user.Username, $"Задача №{issue.Id} {issue.Title} е назначена на вас", $"Задача №{issue.Id} е назначена на вас<br>" +
                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");

                    await _messageService.SendMessageAsync(_userInfo.SysUserID, user.SysUserId, $"Задача №{issue.Id} {issue.Title} е назначена на вас", $"Задача №{issue.Id} е назначена на вас<br>" +
                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
                }
            }

            _context.Issues.Add(issue);
            await ProcessAddedDocs(model, issue);
            await SaveAsync();

            if (issue.AssignedToSysUserId == null)
            {
                // Ако заявката не е назначена на потребител, но е ескалирана, то изпращаме нотификация
                if (issue.IsLevel3Support)
                {
                    // Ако е L3 поддръжка, пращаме само тази нотификация, като прескачаме Escalated
                    var email = await GetNotificationEmailForCategory(issue.CategoryId);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await _emailService.SendEmailAsync(email, $"Задача №{issue.Id} {issue.Title} е ескалирана при подаване към поддръжка ниво 3", $"Задача №{issue.Id} е ескалирана към поддръжка ниво 3" +
                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
                    }
                }
                else if (issue.IsEscalated)
                {
                    string email = await _configurationService.GetValueByKey(Level2NotificationEmailKey);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await _emailService.SendEmailAsync(email, $"Задача №{issue.Id} {issue.Title} е ескалирана при подаване", $"Задача №{issue.Id} {issue.Title} е ескалирана." +
                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
                    }
                }
            }

            return issue.Id;
        }

        public async Task Update(IssueModel model)
        {
            Issue issue = await _context.Issues
                            .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue), nameof(Issue));
            }

            if (issue.StatusId == (int)StatusEnum.Resolved)
            {
                throw new InvalidOperationException(Messages.AlreadyResolved);
            }

            List<string> updateInfo = new List<string>();
            if (issue.CategoryId != model.CategoryId)
            {
                var newCategory = await _context.Categories.FirstOrDefaultAsync(i => i.Id == model.CategoryId);
                var dbCategory = await _context.Categories.FirstOrDefaultAsync(i => i.Id == issue.CategoryId);
                updateInfo.Add($"категория '{dbCategory.Name}'=>'{newCategory.Name}'");
            }
            issue.CategoryId = model.CategoryId;
            issue.SubcategoryId = model.SubcategoryId;
            issue.StatusId = model.StatusId ?? (int)StatusEnum.New;
            issue.PriorityId = model.PriorityId;
            issue.Title = model.Title;
            issue.Phone = model.Phone;
            issue.Description = model.Description;
            issue.Survey = model.Survey;
            if (issue.RequestForInformation != model.RequestForInformation)
            {
                updateInfo.Add(model.RequestForInformation
                        ? "заявката е в очакване на информация от подателя"
                        : "премахнато е очакването на информация от подателя");
            }
            issue.RequestForInformation = model.RequestForInformation;
            bool isEscalated = (model.IsEscalated && !issue.IsEscalated);
            bool isLevel3Support = (model.IsLevel3Support && !issue.IsLevel3Support);
            string username = await GetUsernameForUser(model.AssignedToSysUserId);

            if (issue.IsEscalated != model.IsEscalated)
            {
                updateInfo.Add(model.IsEscalated
                        ? Messages.IssueEscalated
                        : Messages.IssueDeescalated);
            }
            issue.IsEscalated = model.IsEscalated;

            if (issue.IsLevel3Support != model.IsLevel3Support)
            {
                updateInfo.Add(model.IsLevel3Support
                        ? Messages.IssueL3Escalated
                        : Messages.IssueL3Deescalated);
            }
            issue.IsLevel3Support = model.IsLevel3Support;

            if (issue.AssignedToSysUserId == null && model.AssignedToSysUserId != null)
            {
                issue.StatusId = (int)StatusEnum.Assigned;
            }

            if (issue.AssignedToSysUserId != null && model.AssignedToSysUserId == null)
            {
                issue.StatusId = (int)StatusEnum.New;
            }

            if (issue.AssignedToSysUserId != model.AssignedToSysUserId)
            {
                issue.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = issue.StatusId,
                    Comment = model.AssignedToSysUserId.HasValue
                        ? string.Format(Messages.AssignToSysUser, username)
                        : string.Format(Messages.RemoveAssignmentToSysUser, username)
                });
            }
            issue.AssignedToSysUserId = model.AssignedToSysUserId;

            if (updateInfo.Count > 0)
            {
                string updateMessage = String.Join("/", updateInfo);
                issue.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = issue.StatusId,
                    Comment = $"Редакция {updateMessage}"
                }); ;
            }

            using var transaction = _context.Database.BeginTransaction();

            await ProcessAddedDocs(model, issue);
            await ProcessDeletedDocs(model, issue);

            EntityState currentState = _context.Entry(issue).State;
            if (currentState == EntityState.Modified)
            {
                issue.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = issue.StatusId,
                    Comment = Messages.UpdateIssue,
                    IsHidden = true
                });
            }

            await SaveAsync();
            await transaction.CommitAsync();

            if (isEscalated)
            {
                string email = await _configurationService.GetValueByKey(Level2NotificationEmailKey);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendEmailAsync(email, $"Задача №{issue.Id} {issue.Title} е ескалирана", $"Задача №{issue.Id} {issue.Title} е ескалирана." +
                        $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
                }
            }
            if (isLevel3Support)
            {
                var email = await GetNotificationEmailForCategory(issue.CategoryId);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendEmailAsync(email, $"Задача №{issue.Id} {issue.Title} е ескалирана към поддръжка ниво 3", $"Задача №{issue.Id} е ескалирана към поддръжка ниво 3" +
                        $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
                }
            }
        }

        public async Task Reopen(IssueReopenModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(IssueReopenModel));
            }

            Issue issue = await _context.Issues.SingleOrDefaultAsync(d => d.Id == model.IssueId);

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue), nameof(Issue));
            }

            if (issue.StatusId != (int)StatusEnum.Resolved)
            {
                throw new InvalidOperationException(Messages.NotResolved);
            }

            issue.ResolveDate = DateTime.UtcNow;
            issue.StatusId = issue.AssignedToSysUserId != null ? (int)StatusEnum.Assigned : (int)StatusEnum.New;

            issue.IssueStatusHistories.Add(new IssueStatusHistory()
            {
                StatusId = issue.StatusId,
                Comment = string.Format(Messages.Reopen)
            });

            await SaveAsync();
        }

        public async Task Resolve(IssueCommentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(IssueCommentModel));
            }

            Issue issue = await _context.Issues.SingleOrDefaultAsync(d => d.Id == model.IssueId);

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue), nameof(Issue));
            }

            if (issue.StatusId == (int)StatusEnum.Resolved)
            {
                throw new InvalidOperationException(Messages.AlreadyResolved);
            }

            if (!string.IsNullOrWhiteSpace(model.Comment))
            {
                IssueComment issueComment = new IssueComment
                {
                    IssueId = issue.Id,
                    Comment = model.Comment,
                    IsResolvingComment = true
                };

                _context.IssueComments.Add(issueComment);
                await ProcessAddedDocs(model, issueComment);

                issue.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    StatusId = issue.StatusId,
                    Comment = string.Format(Messages.AddComment),
                    IsHidden = true
                });
            }

            issue.ResolveDate = DateTime.UtcNow;
            issue.StatusId = (int)StatusEnum.Resolved;

            var allUsers = await GetAllUsersForIssue(issue.Id);
            var allEmails = await GetEmailsForUsers((allUsers.Except(new List<int>() { _userInfo.SysUserID })).ToList());
            foreach (var email in allEmails)
            {
                await _emailService.SendEmailAsync(email, $"Затворена задача №{issue.Id} {issue.Title}", $"Задача №{issue.Id} е затворена с коментар<br>{model.Comment}" +
                                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
            }

            foreach (var userId in (allUsers.Except(new List<int>() { _userInfo.SysUserID })))
            {
                await _messageService.SendMessageAsync(_userInfo.SysUserID, userId, $"Затворена задача №{issue.Id} {issue.Title}", $"Задача №{issue.Id} е затворена с коментар<br>{model.Comment}" +
                                            $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
            }

            issue.IssueStatusHistories.Add(new IssueStatusHistory()
            {
                StatusId = issue.StatusId,
                Comment = issue.StatusId == (int)StatusEnum.Resolved
                    ? Messages.Resolved
                    : string.Format(Messages.AssignToSysUser, await GetUsernameForUser(issue.AssignedToSysUserId))
            });

            await SaveAsync();
        }

        public async Task Comment(IssueCommentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(IssueCommentModel));
            }

            Issue issue = await _context.Issues.SingleOrDefaultAsync(d => d.Id == model.IssueId);

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue), nameof(Issue));
            }

            if (issue.StatusId == (int)StatusEnum.Resolved)
            {
                throw new InvalidOperationException(Messages.AlreadyResolved);
            }

            if (!string.IsNullOrWhiteSpace(model.Comment))
            {
                var issueComment = new IssueComment
                {
                    IssueId = issue.Id,
                    Comment = model.Comment
                };

                _context.IssueComments.Add(issueComment);
                await ProcessAddedDocs(model, issueComment);

                _context.IssueStatusHistories.Add(new IssueStatusHistory()
                {
                    IssueId = issue.Id,
                    StatusId = issue.StatusId,
                    Comment = string.Format(Messages.AddComment),
                    IsHidden = true
                });
            }

            var allUsers = await GetAllUsersForIssue(issue.Id);
            var allEmails = await GetEmailsForUsers((allUsers.Except(new List<int>() { _userInfo.SysUserID })).ToList());
            foreach (var email in allEmails)
            {
                await _emailService.SendEmailAsync(email, $"Добавен коментар към задача №{issue.Id} {issue.Title}", $"Към задача №{issue.Id} е добавен коментар<br> \"{model.Comment}\"" +
                        $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
            }

            foreach (var userId in (allUsers.Except(new List<int>() { _userInfo.SysUserID })))
            {
                await _messageService.SendMessageAsync(_userInfo.SysUserID, userId, $"Добавен коментар към задача №{issue.Id} {issue.Title}", $"Към задача №{issue.Id} е добавен коментар<br> \"{model.Comment}\"" +
                    $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");
            }

            await SaveAsync();
        }

        public async Task AssignTo(IssueAssignmentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(IssueAssignmentModel));
            }

            Issue issue = await _context.Issues.SingleOrDefaultAsync(d => d.Id == model.IssueId);

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue), nameof(Issue));
            }

            if (issue.StatusId == (int)StatusEnum.Resolved)
            {
                throw new InvalidOperationException(Messages.AlreadyResolved);
            }

            var user = await _context.SysUsers
                .Where(x => x.SysUserId == model.UserId)
                .Select(x => new { x.SysUserId, x.Username })
                .FirstOrDefaultAsync();

            if (user != null && user.SysUserId == issue.AssignedToSysUserId)
            {
                throw new InvalidOperationException(Messages.AlreadyAssigned);
            }

            issue.AssignedToSysUserId = model.UserId;
            issue.StatusId = model.UserId.HasValue ? (int)StatusEnum.Assigned : (int)StatusEnum.New;
            issue.IssueStatusHistories.Add(new IssueStatusHistory()
            {
                StatusId = issue.StatusId,
                Comment = model.UserId.HasValue
                    ? string.Format(Messages.AssignToSysUser, user?.Username)
                    : string.Format(Messages.RemoveAssignmentToSysUser, user?.Username)
            });

            await _emailService.SendEmailAsync(user.Username, $"Задача №{issue.Id} {issue.Title} е назначена на вас", $"Задача №{issue.Id} е назначена на вас<br>" +
                    $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");

            await _messageService.SendMessageAsync(_userInfo.SysUserID, user.SysUserId, $"Задача №{issue.Id} {issue.Title} е назначена на вас", $"Задача №{issue.Id} е назначена на вас<br>" +
                    $"<br>Вижте повече: <a href='https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}'>https://helpdesk-neispuo.mon.bg/Issue/{issue.Id}</a>");

            await SaveAsync();
        }

        public Task AssignToMyself(IssueAssignmentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(IssueAssignmentModel));
            }

            model.UserId = _userInfo.SysUserID;
            return AssignTo(model);
        }

        public async Task LogReadActivity(int? id)
        {
            if (!id.HasValue) return;

            var entity = await _context.IssueReadActivities
                .SingleOrDefaultAsync(x => x.IssueId == id && x.CreatedBySysUserId == _userInfo.SysUserID);

            if (entity == null)
            {
                _context.IssueReadActivities.Add(new IssueReadActivity
                {
                    IssueId = id.Value
                });
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await SaveAsync();
        }
    }
}
