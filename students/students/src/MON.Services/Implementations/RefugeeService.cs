namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Grid;
    using MON.Models.Refugee;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class RefugeeService : BaseService<RefugeeService>, IRefugeeService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IInstitutionService _institutionService;
        private readonly IStudentService _studentService;

        public RefugeeService(DbServiceDependencies<RefugeeService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IInstitutionService institutionService,
            IStudentService studentService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _institutionService = institutionService;
            _studentService = studentService;
        }

        public Task<ApplicationModel> GetByIdAsync(int id)
        {
            return (
                from x in _context.Applications
                where x.Id == id
                select new ApplicationModel()
                {
                    Id = x.Id,
                    ApplicantFullName = x.ApplicantFullName,
                    ApplicationDate = x.ApplicationDate,
                    TownId = x.TownId,
                    Address = x.Address,
                    RegionId = x.RegionId,
                    NationalityId = x.NationalityId,
                    Email = x.Email,
                    Phone = x.Phone,
                    PersonalId = x.PersonalId,
                    Status = x.Status,
                    PersonalIdTypeModel = new DropdownViewModel
                    {
                        Value = x.PersonalIdtype,
                        Text = x.PersonalIdtypeNavigation.Name,
                        Name = x.PersonalIdtypeNavigation.Name
                    },
                    PersonalIdType = x.PersonalIdtype,
                    GuardianType = x.GuardianTypeId,
                    Children = x.ApplicationChildren.Select(c => new ApplicationChildModel()
                    {
                        Id = c.Id,
                        PersonId = c.PersonId,
                        FirstName = c.FirstName,
                        MiddleName = c.MiddleName,
                        LastName = c.LastName,
                        BgLanguageSkill = c.BglanguageSkill,
                        EnLanguageSkill = c.EnlanguageSkill,
                        DeLanguageSkill = c.DelanguageSkill,
                        FrLanguageSkill = c.FrlanguageSkill,
                        OtherLanguage = c.OtherLanguageName,
                        OtherLanguageSkill = c.OtherLanguageSkill,
                        BirthDate = c.BirthDate,
                        Gender = c.GenderId,
                        Email = c.Email,
                        Phone = c.Phone,
                        HasNeedForResourceSupport = c.HasNeedForResourceSupport,
                        HasNeedForTextbooks = c.HasNeedForTextbooks,
                        IsForCsop = c.IsForCsop,
                        NationalityId = c.NationalityId.Value,
                        TownId = c.TownId,
                        IsClassCompleted = c.IsClassCompleted,
                        LastBasicClassId = c.LastBasicClassId,
                        PersonalId = c.PersonalId,
                        PersonalIdType = c.PersonalIdtype,
                        PersonalIdTypeModel = new DropdownViewModel
                        {
                            Value = c.PersonalIdtype,
                            Text = c.PersonalIdtypeNavigation.Name,
                            Name = c.PersonalIdtypeNavigation.Name
                        },
                        LastInstitutionType = c.LastInstiutionType,
                        LastInstitutionCountry = c.LastInstitutionCountry,
                        Profession = c.Profession,
                        Address = c.Address,
                        RuoDocNumber = c.RuodocNumber,
                        RuoDocDate = c.RuodocDate,
                        ProtectionStatus = c.ProtectionStatus,
                        SchoolYear = c.SchoolYear,
                        InstitutionId = c.InstitutionId,
                        HasDualCitizenship = c.HasDualCitizenship,
                        HasDocumentForCompletedClass = c.HasDocumentForCompletedClass,
                        Status = c.Status,
                        StatusName = c.StatusName
                    }).ToList(),
                    Documents = x.ApplicationDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).FirstOrDefaultAsync();
        }

        public Task<ApplicationViewModel> GetDetailsByIdAsync(int id)
        {
            return (
                from x in _context.Applications
                where x.Id == id
                select new ApplicationViewModel()
                {
                    Id = x.Id,
                    ApplicantFullName = x.ApplicantFullName,
                    ApplicationDate = x.ApplicationDate,
                    Town = string.Join(", ", $"гр./с.{x.Town.Name}", $"общ.{x.Town.Municipality.Name}", $"обл.{x.Town.Municipality.Region.Name}"),
                    Address = x.Address,
                    Region = x.Region.Name,
                    Nationality = x.Nationality.Name,
                    GuardianTypeName = x.GuardianType.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    PersonalId = x.PersonalId,
                    PersonalIdTypeName = x.PersonalIdtypeNavigation.Name,
                    Status = x.Status,
                    Children = x.ApplicationChildren.Select(c => new ApplicationChildViewModel()
                    {
                        Id = c.Id,
                        PersonId = c.PersonId,
                        FirstName = c.FirstName,
                        MiddleName = c.MiddleName,
                        LastName = c.LastName,
                        BgLanguageSkill = c.BglanguageSkill,
                        EnLanguageSkill = c.EnlanguageSkill,
                        DeLanguageSkill = c.DelanguageSkill,
                        FrLanguageSkill = c.FrlanguageSkill,
                        OtherLanguage = c.OtherLanguageName,
                        OtherLanguageSkill = c.OtherLanguageSkill,
                        BirthDate = c.BirthDate,
                        GenderName = c.Gender.Name,
                        Email = c.Email,
                        Phone = c.Phone,
                        HasNeedForResourceSupport = c.HasNeedForResourceSupport,
                        IsForCsop = c.IsForCsop,
                        HasNeedForTextbooks = c.HasNeedForTextbooks,
                        Nationality = c.Nationality.Name,
                        Town = string.Join(", ", $"гр./с.{c.Town.Name}", $"общ.{c.Town.Municipality.Name}", $"обл.{c.Town.Municipality.Region.Name}"),
                        IsClassCompleted = c.IsClassCompleted,
                        LastBasicClassId = c.LastBasicClassId,
                        LastBasicClassName = c.LastBasicClass.Description,
                        PersonalId = c.PersonalId,
                        PersonalIdTypeName = c.PersonalIdtypeNavigation.Name,
                        LastInstitutionType = c.LastInstiutionType,
                        LastInstitutionCountry = c.LastInstitutionCountry,
                        LastInstitutionCountryName = c.LastInstitutionCountryNavigation.Name,
                        Profession = c.Profession,
                        ProtectionStatus = c.ProtectionStatus,
                        Address = c.Address,
                        RuoDocNumber = c.RuodocNumber,
                        RuoDocDate = c.RuodocDate,
                        SchoolYear = c.SchoolYear,
                        Institution = $"{c.InstitutionId}.{c.InstitutionSchoolYear.Name} гр./с.{c.InstitutionSchoolYear.Town.Name} общ.{c.InstitutionSchoolYear.Town.Municipality.Name} обл.{c.InstitutionSchoolYear.Town.Municipality.Region.Name}",
                        HasDualCitizenship = c.HasDualCitizenship,
                        HasDocumentForCompletedClass = c.HasDocumentForCompletedClass,
                        Status = c.Status,
                        StatusName = c.StatusName
                    }).ToList(),
                    Documents = x.ApplicationDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).FirstOrDefaultAsync();
        }

        public Task<int> CountPendingAdmissions()
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                return Task.FromResult(0);
            }

            return _context.VRefugeeAdmissionDetails
                .CountAsync(x => x.PersonId != null && x.InstitutionId == _userInfo.InstitutionID && x.CurrentInstitutionId == null);
        }

        public async Task<IPagedList<RefugeeAdmissionViewModel>> AdmissionList(PagedListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);
            }

            IQueryable<VRefugeeAdmissionDetail> listQuery = _context.VRefugeeAdmissionDetails
                .AsNoTracking()
                .Where(x => x.PersonId != null);

            if (_userInfo.InstitutionID.HasValue)
            {
                listQuery = listQuery.Where(x => x.InstitutionId != null && x.InstitutionId == _userInfo.InstitutionID
                    && (x.CurrentInstitutionId == null || x.CurrentInstitutionId == _userInfo.InstitutionID));
            }
            else if (_userInfo.IsRuo)
            {
                listQuery = listQuery.Where(x => _userInfo.RegionID.HasValue && x.InstitutioRegionId == _userInfo.RegionID);
            }

            listQuery = listQuery.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                    || predicate.Pin.Contains(input.Filter)
                    || predicate.PinType.Contains(input.Filter)
                    || predicate.InstitutionCode.Contains(input.Filter)
                    || predicate.InstitutionAbbreviation.Contains(input.Filter)
                    || predicate.InstitutioTown.Contains(input.Filter)
                    || predicate.InstitutioRegion.Contains(input.Filter)
                    || predicate.Classes.Contains(input.Filter));

            IQueryable<RefugeeAdmissionViewModel> query = listQuery
                .Select(x => new RefugeeAdmissionViewModel
                {
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    Pin = x.Pin,
                    PinType = x.PinType,
                    Age = x.Age,
                    InstitutionCode = x.InstitutionCode,
                    InstitutionName = x.InstitutionAbbreviation,
                    InstitutionTown = x.InstitutioTown,
                    InstitutioRegion = x.InstitutioRegion,
                    AdmissionStatus = x.AdmissionStatus,
                    Classes = x.Classes,
                    ApplicationId = x.ApplicationId,
                    CurrentInstitutionId = x.CurrentInstitutionId,
                    RuodocNumber = x.RuodocNumber,
                    RuodocDate = x.RuodocDate
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "AdmissionStatus desc, FullName asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<RefugeeAdmissionViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }
        public async Task<IPagedList<ApplicationViewModel>> ApplicationList(RefugeeApplicationListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);
            }

            IQueryable<Application> query = _context.Applications
                .AsNoTracking();

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RegionId == _userInfo.RegionID.Value);
            }

            IQueryable<ApplicationViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.ApplicantFullName.Contains(input.Filter)
                   || predicate.Nationality.Name.Contains(input.Filter)
                   || predicate.Region.Name.Contains(input.Filter)
                   || predicate.PersonalId.Contains(input.Filter)
                   || predicate.StatusName.Contains(input.Filter))
                .Select(x => new ApplicationViewModel
                {
                    Id = x.Id,
                    ApplicantFullName = x.ApplicantFullName,
                    Address = x.Address,
                    PersonalId = x.PersonalId,
                    Region = x.Region.Name,
                    Nationality = x.Nationality.Name,
                    ApplicationDate = x.ApplicationDate,
                    Status = x.Status,
                    StatusName = x.StatusName,
                    Children = x.ApplicationChildren.OrderBy(x => x.FirstName).Select(x => new ApplicationChildViewModel
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        PersonalId = x.PersonalId,
                        PersonalIdTypeName = x.PersonalIdtypeNavigation.Name,
                        InstitutionId = x.InstitutionId,
                        Institution = x.InstitutionSchoolYear.Abbreviation,
                        StatusName = x.StatusName,
                        Status = x.Status,
                        RuoDocNumber = x.RuodocNumber,
                        RuoDocDate = x.RuodocDate
                    }).ToList(),
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "ApplicationDate desc, Status asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync();
            IList<ApplicationViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<int> CreateApplication(ApplicationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var entry = new Application
            {
                ApplicantFullName = model.ApplicantFullName,
                ApplicationDate = model.ApplicationDate,
                TownId = model.TownId,
                NationalityId = model.NationalityId,
                Email = model.Email,
                Phone = model.Phone,
                RegionId = _userInfo.RegionID.Value,
                PersonalId = model.PersonalId,
                PersonalIdtype = model.PersonalIdTypeModel.Value,
                Address = model.Address,
                GuardianTypeId = model.GuardianType,
                Status = (int)ApplicationStatusEnum.InProcess,
                ApplicationChildren = model.Children.Select(c => new ApplicationChild()
                {
                    FirstName = c.FirstName,
                    MiddleName = c.MiddleName,
                    LastName = c.LastName,
                    AddressCoincidesWithApplication = false,
                    BglanguageSkill = c.BgLanguageSkill,
                    EnlanguageSkill = c.EnLanguageSkill,
                    DelanguageSkill = c.DeLanguageSkill,
                    FrlanguageSkill = c.FrLanguageSkill,
                    OtherLanguageName = c.OtherLanguage,
                    OtherLanguageSkill = c.OtherLanguageSkill,
                    BirthDate = c.BirthDate,
                    GenderId = c.Gender,
                    Email = c.Email,
                    Phone = c.Phone,
                    HasNeedForResourceSupport = c.HasNeedForResourceSupport,
                    HasNeedForTextbooks = c.HasNeedForTextbooks,
                    IsForCsop = c.IsForCsop,
                    NationalityId = c.NationalityId,
                    TownId = c.TownId,
                    IsClassCompleted = c.IsClassCompleted,
                    LastBasicClassId = c.LastBasicClassId,
                    PersonalId = c.PersonalId,
                    PersonalIdtype = c.PersonalIdTypeModel.Value,
                    LastInstiutionType = c.LastInstitutionType,
                    LastInstitutionCountry = c.LastInstitutionCountry,
                    Profession = c.Profession,
                    Address = c.Address,
                    RuodocNumber = c.RuoDocNumber,
                    RuodocDate = c.RuoDocDate,
                    ProtectionStatus = c.ProtectionStatus,
                    InstitutionId = c.InstitutionId,
                    SchoolYear = c.InstitutionId.HasValue
                        ? _institutionService.GetCurrentYear(c.InstitutionId.Value).Result
                        : (short?)null,
                    HasDualCitizenship = c.HasDualCitizenship,
                    HasDocumentForCompletedClass = c.HasDocumentForCompletedClass,
                    Status = (int)ApplicationStatusEnum.InProcess,
                }).ToList()
            };

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);
            _context.Applications.Add(entry);
            await SaveAsync();
            await transaction.CommitAsync();

            return entry.Id;
        }

        public async Task UpdateApplication(ApplicationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            Application entity = await _context.Applications
                .Include(d => d.ApplicationChildren)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), nameof(Application));
            }

            UpdateApplicationEntity(model, entity);

            // За изтриване
            List<ApplicationChild> applicationChildrenToDelete = entity.ApplicationChildren
                .Where(x => model.Children == null || !model.Children.Any(c => c.Id.HasValue && c.Id.Value == x.Id))
                .ToList();
            if (applicationChildrenToDelete.Any())
            {
                _context.ApplicationChildren.RemoveRange(applicationChildrenToDelete);
            }

            if (!model.Children.IsNullOrEmpty())
            {
                // За добавяне
                foreach (var c in model.Children.Where(x => !x.Id.HasValue))
                {
                    entity.ApplicationChildren.Add(new ApplicationChild()
                    {
                        FirstName = c.FirstName,
                        MiddleName = c.MiddleName,
                        LastName = c.LastName,
                        AddressCoincidesWithApplication = false,
                        BglanguageSkill = c.BgLanguageSkill,
                        EnlanguageSkill = c.EnLanguageSkill,
                        DelanguageSkill = c.DeLanguageSkill,
                        FrlanguageSkill = c.FrLanguageSkill,
                        OtherLanguageName = c.OtherLanguage,
                        OtherLanguageSkill = c.OtherLanguageSkill,
                        BirthDate = c.BirthDate,
                        GenderId = c.Gender,
                        Email = c.Email,
                        Phone = c.Phone,
                        HasNeedForResourceSupport = c.HasNeedForResourceSupport,
                        HasNeedForTextbooks = c.HasNeedForTextbooks,
                        NationalityId = c.NationalityId,
                        TownId = c.TownId,
                        IsClassCompleted = c.IsClassCompleted,
                        LastBasicClassId = c.LastBasicClassId,
                        PersonalId = c.PersonalId,
                        PersonalIdtype = c.PersonalIdTypeModel.Value,
                        LastInstiutionType = c.LastInstitutionType,
                        LastInstitutionCountry = c.LastInstitutionCountry,
                        Profession = c.Profession,
                        Address = c.Address,
                        RuodocNumber = c.RuoDocNumber,
                        RuodocDate = c.RuoDocDate,
                        ProtectionStatus = c.ProtectionStatus,
                        InstitutionId = c.InstitutionId,
                        SchoolYear = c.InstitutionId.HasValue
                            ? _institutionService.GetCurrentYear(c.InstitutionId.Value).Result
                            : (short?)null,
                        HasDualCitizenship = c.HasDualCitizenship,
                        HasDocumentForCompletedClass = c.HasDocumentForCompletedClass,
                        Status = (int)ApplicationStatusEnum.InProcess,
                    });
                }

                // За редакция
                foreach (var source in model.Children.Where(x => x.Id.HasValue))
                {
                    ApplicationChild target = entity.ApplicationChildren.SingleOrDefault(x => x.Id == source.Id);
                    if (target == null && target.Status != (int)ApplicationStatusEnum.InProcess) continue;

                    UpdateApplicationChildEntity(source, target);
                }
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        /// <summary>
        /// Маркиране на заявление като обработено.
        /// Ще маркира като обработени и всичките му ApplicationChildren-и, които не са анулирани.
        /// Необходимо е всчки ApplicationChildren-и, които не са анулирани, да имат въведени RUODocNumber, RUODocDate и InstitutionId.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task CompleteApplication(int applicationId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            Application application = await _context.Applications
                .Include(x => x.ApplicationChildren)
                .Where(x => x.Id == applicationId)
                .SingleOrDefaultAsync();

            if (application == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (application.Status == (int)ApplicationStatusEnum.Completed)
            {
                throw new ApiException(Messages.AlreadyCompleted);
            }

            if (!application.ApplicationChildren.Any())
            {
                throw new ApiException(Messages.RefugeeRequestMissingChildren);
            }

            if (application.ApplicationChildren.Any(x => x.Status == (int)ApplicationStatusEnum.InProcess
                && !IsApplicationChildValidForCompletion(x)))
            {
                throw new ApiException(Messages.RefugeeRequestChildWithMissingRuoAttrs);
            }

            foreach (ApplicationChild appChild in application.ApplicationChildren.Where(x => x.Status == (int)ApplicationStatusEnum.InProcess))
            {
                if (!IsApplicationChildValidForCompletion(appChild))
                {
                    throw new ApiException(Messages.RefugeeRequestChildWithMissingRuoAttrs);
                }

                await UpdateApplicationChildPerson(appChild);

                appChild.Status = (int)ApplicationStatusEnum.Completed;
            }

            application.Status = (int)ApplicationStatusEnum.Completed;
            await SaveAsync();
        }

        public async Task CompleteApplicationChild(int applicationChildId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApplicationChild applicationChild = await _context.ApplicationChildren
                .Include(x => x.Application)
                .Where(x => x.Id == applicationChildId)
                .SingleOrDefaultAsync();

            if (applicationChild == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (applicationChild.Status == (int)ApplicationStatusEnum.Completed)
            {
                throw new ApiException(Messages.AlreadyCompleted);
            }

            if (applicationChild.Application.Status != (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException("Заявлението не е 'В процес на обработка'");
            }

            if (!IsApplicationChildValidForCompletion(applicationChild))
            {
                throw new ApiException(Messages.RefugeeRequestChildWithMissingRuoAttrs);
            }

            await UpdateApplicationChildPerson(applicationChild);
            applicationChild.Status = (int)ApplicationStatusEnum.Completed;
            await SaveAsync();

            if (!await _context.ApplicationChildren.AnyAsync(x => x.ApplicationId == applicationChild.ApplicationId && x.Status == (int)ApplicationStatusEnum.InProcess))
            {
                applicationChild.Application.Status = (int)ApplicationStatusEnum.Completed;
                await SaveAsync();
            }
        }

        private async Task UpdateApplicationChildPerson(ApplicationChild appChild)
        {
            if (appChild == null) throw new ArgumentNullException(nameof(ApplicationChild));

            // ApplicationChild entity-то има PersonId
            if (appChild.PersonId.HasValue) return;

            // Проверка дали такъв ученик(Person) вече съществува
            var person = await _context.People
                .Where(x => x.PersonalId == appChild.PersonalId.Trim() && x.PersonalIdtype == appChild.PersonalIdtype)
                .Select(x => new { x.PersonId })
                .FirstOrDefaultAsync();

            if (person != null)
            {
                appChild.PersonId = person.PersonId;
                return;
            }

            var studentModel = new StudentCreateModel()
            {
                FirstName = appChild.FirstName,
                MiddleName = appChild.MiddleName,
                LastName = appChild.LastName,
                BirthDate = appChild.BirthDate,
                PermanentResidenceId = appChild.TownId,
                PermanentAddress = appChild.Address,
                UsualResidenceId = appChild.TownId,
                CurrentAddress = appChild.Address,
                Pin = appChild.PersonalId,
                PinTypeId = appChild.PersonalIdtype,
                NationalityId = appChild.NationalityId,
                GenderId = appChild.GenderId,
            };

            int personId = await _studentService.AddAsync(studentModel);
            appChild.PersonId = personId;

            if (appChild.ProtectionStatus != (int)RefugeeProtectionStatusEnum.NoProtection)
            {
                var internationalProtection =
                new InternationalProtection()
                {
                    ProtectionStatus = appChild.ProtectionStatus,
                    DocDate = appChild.RuodocDate,
                    DocNumber = appChild.RuodocNumber,
                    PersonId = personId
                };

                _context.InternationalProtections.Add(internationalProtection);
            };
        }

        public async Task CancelApplication(RefugeeApplicationCancellationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            int? applicationId = model?.ApplicationId;
            Application application = await _context.Applications
               .Include(x => x.ApplicationChildren)
               .SingleOrDefaultAsync(d => d.Id == applicationId);

            if (application == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (application.Status == (int)ApplicationStatusEnum.Cancelled)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Already cancelled."));
            }

            DateTime utcNow = DateTime.UtcNow;
            application.Status = (int)ApplicationStatusEnum.Cancelled;
            application.CancelledBySysUserId = _userInfo.SysUserID;
            application.CancellationDate = utcNow;
            foreach (ApplicationChild child in application.ApplicationChildren.Where(x => x.Status == null || x.Status != ((int)ApplicationStatusEnum.Cancelled)))
            {
                child.Status = (int)ApplicationStatusEnum.Cancelled;
                child.CancelledBySysUserId = _userInfo.SysUserID;
                child.CancellationDate = utcNow;
            }
            await SaveAsync();
        }

        public async Task CancelApplicationChild(RefugeeApplicationCancellationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            int? applicationChildId = model?.ApplicationChildId;
            ApplicationChild applicationChild = await _context.ApplicationChildren
                .Include(x => x.Application)
                .SingleOrDefaultAsync(x => x.Id == applicationChildId);

            if (applicationChild == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (applicationChild.Status.HasValue && applicationChild.Status == (int)ApplicationStatusEnum.Cancelled)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Application child has been already cancelled."));
            }

            if (applicationChild.Application.Status == (int)ApplicationStatusEnum.Cancelled)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Application has been already cancelled."));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            DateTime utcNow = DateTime.UtcNow;
            applicationChild.Status = (int)ApplicationStatusEnum.Cancelled;
            applicationChild.CancelledBySysUserId = _userInfo.SysUserID;
            applicationChild.CancellationDate = utcNow;

            await SaveAsync();

            if (!await _context.ApplicationChildren.AnyAsync(x => x.ApplicationId == applicationChild.ApplicationId && x.Status != (int)ApplicationStatusEnum.Cancelled))
            {
                // В заявката на това дете няма деца, които да не са анулирани. Тога анулираме и самата заявка.
                applicationChild.Application.Status = (int)ApplicationStatusEnum.Cancelled;
                applicationChild.Application.CancelledBySysUserId = _userInfo.SysUserID;
                applicationChild.Application.CancellationDate = utcNow;
                await SaveAsync();
            }

            await transaction.CommitAsync();
        }

        public async Task DeleteApplication(int applicationId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            Application application = await _context.Applications
                .Include(d => d.ApplicationChildren)
                .Include(d => d.ApplicationDocuments).ThenInclude(x => x.Document)
                .SingleOrDefaultAsync(d => d.Id == applicationId);

            if (application == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (application.Status != (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Not in progress state."));
            }

            _context.ApplicationDocuments.RemoveRange(application.ApplicationDocuments);
            _context.Documents.RemoveRange(application.ApplicationDocuments.Select(x => x.Document));
            _context.ApplicationChildren.RemoveRange(application.ApplicationChildren);
            _context.Applications.Remove(application);

            await SaveAsync();
        }

        public async Task DeleteApplicationChild(int applicationChildId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApplicationChild applicationChild = await _context.ApplicationChildren
                .Include(x => x.Application)
                .SingleOrDefaultAsync(x => x.Id == applicationChildId);


            if (applicationChild == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (applicationChild.Status.HasValue && applicationChild.Status.Value != (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Not in progress state."));
            }

            if (applicationChild.Application.Status != (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Application not in progress state."));
            }

            _context.ApplicationChildren.RemoveRange(applicationChild);

            await SaveAsync();
        }

        public async Task UnlockApplication(int applicationId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            Application application = await _context.Applications
                .Include(x => x.ApplicationChildren)
                .SingleOrDefaultAsync(x => x.Id == applicationId);

            if (application == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (application.Status == (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Already in progress."));
            }

            application.Status = (int)ApplicationStatusEnum.InProcess;

            foreach (var child in application.ApplicationChildren.Where(x => x.Status == (int)ApplicationStatusEnum.Completed))
            {
                child.Status = (int)ApplicationStatusEnum.InProcess;
            }

            await SaveAsync();
        }

        public async Task UnlockApplicationChild(int applicationChildId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRefugeeApplicationsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApplicationChild applicationChild = await _context.ApplicationChildren
                .Include(x => x.Application)
                .SingleOrDefaultAsync(x => x.Id == applicationChildId);

            if (applicationChild == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (applicationChild.Status == (int)ApplicationStatusEnum.InProcess)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Already in progress."));
            }

            applicationChild.Status = (int)ApplicationStatusEnum.InProcess;
            if (applicationChild.Application.Status != (int)ApplicationStatusEnum.InProcess)
            {
                applicationChild.Application.Status = (int)ApplicationStatusEnum.InProcess;
            }
            await SaveAsync();
        }

        private async Task ProcessAddedDocs(ApplicationModel model, Application entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.ApplicationDocuments.Add(new ApplicationDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(ApplicationModel model, Application dischargeDocument)
        {
            if (model.Documents == null || !model.Documents.Any() || dischargeDocument == null) return;

            HashSet<int> docIdsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true)
                .Select(x => x.Id.Value).ToHashSet();

            await _context.ApplicationDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        private void UpdateApplicationEntity(ApplicationModel model, Application entity)
        {
            if (model == null || entity == null) return;

            entity.ApplicantFullName = model.ApplicantFullName;
            entity.ApplicationDate = model.ApplicationDate;
            entity.TownId = model.TownId;
            entity.NationalityId = model.NationalityId;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.PersonalId = model.PersonalId;
            entity.PersonalIdtype = model.PersonalIdTypeModel.Value;
            entity.Address = model.Address;
            entity.GuardianTypeId = model.GuardianType;
        }

        private void UpdateApplicationChildEntity(ApplicationChildModel model, ApplicationChild entity)
        {
            if (model == null || entity == null) return;

            entity.FirstName = model.FirstName;
            entity.MiddleName = model.MiddleName;
            entity.LastName = model.LastName;
            entity.BglanguageSkill = model.BgLanguageSkill;
            entity.EnlanguageSkill = model.EnLanguageSkill;
            entity.DelanguageSkill = model.DeLanguageSkill;
            entity.FrlanguageSkill = model.FrLanguageSkill;
            entity.OtherLanguageName = model.OtherLanguage;
            entity.OtherLanguageSkill = model.OtherLanguageSkill;
            entity.BirthDate = model.BirthDate;
            entity.GenderId = model.Gender;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.HasNeedForResourceSupport = model.HasNeedForResourceSupport;
            entity.HasNeedForTextbooks = model.HasNeedForTextbooks;
            entity.NationalityId = model.NationalityId;
            entity.TownId = model.TownId;
            entity.IsClassCompleted = model.IsClassCompleted;
            entity.LastBasicClassId = model.LastBasicClassId;
            entity.PersonalId = model.PersonalId;
            entity.PersonalIdtype = model.PersonalIdTypeModel.Value;
            entity.LastInstiutionType = model.LastInstitutionType;
            entity.LastInstitutionCountry = model.LastInstitutionCountry;
            entity.Profession = model.Profession;
            entity.Address = model.Address;
            entity.RuodocNumber = model.RuoDocNumber;
            entity.RuodocDate = model.RuoDocDate;
            entity.InstitutionId = model.InstitutionId;
            entity.SchoolYear = model.InstitutionId.HasValue
                            ? _institutionService.GetCurrentYear(model.InstitutionId.Value).Result
                            : (short?)null;
            entity.ProtectionStatus = model.ProtectionStatus;
            entity.HasDualCitizenship = model.HasDualCitizenship;
            entity.HasDocumentForCompletedClass = model.HasDocumentForCompletedClass;
        }

        private bool IsApplicationChildValidForCompletion(ApplicationChild model)
        {
            return model != null && !model.RuodocNumber.IsNullOrWhiteSpace()
                && model.RuodocDate.HasValue && model.InstitutionId.HasValue;
        }
    }
}
