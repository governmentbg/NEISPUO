using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class StudentScholarshipService : BaseService<StudentScholarshipService>, IStudentScholarshipService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly IInstitutionService _institutionService;
        private readonly IStudentService _studentService;


        public StudentScholarshipService(DbServiceDependencies<StudentScholarshipService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            ILodFinalizationService lodFinalizationService,
            IInstitutionService institutionService,
            IStudentService studentService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _lodFinalizationService = lodFinalizationService;
            _institutionService = institutionService;
            _studentService = studentService;
        }

        public async Task<ScholarshipViewModel> GetById(int id)
        {
            ScholarshipViewModel model = await _context.ScholarshipStudents
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x =>
                    new ScholarshipViewModel
                    {
                        Id = x.Id,
                        PersonId = x.PersonId,
                        Description = x.Description,
                        AmountRate = x.AmountRate,
                        Currency = x.Currency,
                        AltCurrency = x.AltCurrency,
                        AltAmountRate = x.AltAmountRate,
                        SchoolYear = x.SchoolYear,
                        SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                        OrderNumber = x.OrderNumber,
                        OrderDate = x.OrderDate,
                        ScholarshipFinancingOrganId = x.FinancingOrganId,
                        ScholarshipFinancingOrganName = x.FinancingOrgan.Name,
                        InstitutionId = x.InstitutionId,
                        InstitutionName = x.InstitutionSchoolYear.Name,
                        Periodicity = x.Periodicity ?? -1,
                        ScholarshipAmountId = x.ScholarshipAmountId,
                        ScholarshipAmountName = x.ScholarshipAmount.Name,
                        ScholarshipTypeId = x.ScholarshipTypeId,
                        ScholarshipTypeName = x.ScholarshipType.Name,
                        StartingDateOfReceiving = x.ValidFrom,
                        EndDateOfReceiving = x.ValidTo,
                        CommissionDate = x.CommissionDate,
                        CommissionNumber = x.CommissionNumber,
                        Documents = x.ScholarshipStudentDocuments
                            .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                           
                    })
                .SingleOrDefaultAsync();

            if (model != null)
            {
                // Методът се използва при Details и Edit
                if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentScholarshipRead)
                    && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentScholarshipManage))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            return model;
        }

        public async Task<StudentScholarshipsViewModel> GetByPersonId(int personId, int? schoolYear)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentScholarshipRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return new StudentScholarshipsViewModel
            {
                ScholarshipDetails = await (from x in _context.ScholarshipStudents
                                            join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                                            from lodFin in temp.DefaultIfEmpty()
                                            where x.PersonId == personId
                                            orderby x.SchoolYear descending
                                            select new ScholarshipViewModel
                                            {
                                                Id = x.Id,
                                                PersonId = x.PersonId,
                                                Description = x.Description,
                                                AmountRate = x.AmountRate,
                                                Currency = x.Currency,
                                                AltCurrency = x.AltCurrency,
                                                AltAmountRate = x.AltAmountRate,
                                                SchoolYear = x.SchoolYear,
                                                SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                                                OrderNumber = x.OrderNumber,
                                                OrderDate = x.OrderDate,
                                                InstitutionName = x.InstitutionSchoolYear.Name,
                                                ScholarshipFinancingOrganName = x.FinancingOrgan.Description,
                                                Periodicity = x.Periodicity ?? -1,
                                                ScholarshipAmountName = x.ScholarshipAmount.Name,
                                                ScholarshipTypeName = x.ScholarshipType.Name,
                                                StartingDateOfReceiving = x.ValidFrom,
                                                EndDateOfReceiving = x.ValidTo,
                                                CommissionDate = x.CommissionDate,
                                                CommissionNumber = x.CommissionNumber,
                                                Documents = x.ScholarshipStudentDocuments
                                                    .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                                                IsLodFinalized = lodFin != null && lodFin.IsFinalized
                                            })
                                            .ToListAsync()
            };
    }

    public async Task Create(StudentScholarshipModel model)
    {
        if (model == null)
        {
            throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model)));
        }

        if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentScholarshipManage))
        {
            throw new ApiException(Messages.UnauthorizedMessageError, 401);
        }

        int? institutionId = _userInfo?.InstitutionID;
        short schoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(institutionId);

        StudentClassViewModel studentClass = await _studentService.GetMainStudentClass(model.PersonId, true, schoolYear, institutionId);

        var newScholarship = new ScholarshipStudent
        {
            PersonId = model.PersonId,
            StudentClassId = studentClass?.Id,
            SchoolYear = schoolYear,
            InstitutionId = institutionId,
            AmountRate = model.AmountRate,
            ScholarshipAmountId = model.ScholarshipAmountId,
            ScholarshipTypeId = model.ScholarshipTypeId,
            Description = model.Description,
            ValidFrom = model.StartingDateOfReceiving,
            ValidTo = model.EndDateOfReceiving,
            OrderNumber = model.OrderNumber,
            FinancingOrganId = model.ScholarshipFinancingOrganId,
            OrderDate = model.OrderDate,
            Periodicity = model.Periodicity,
            CommissionDate = model.CommissionDate,
            CommissionNumber = model.CommissionNumber,
            Currency = (await GetMainCurrency()).Code,
        };

        await ProcessAddedDocs(model, newScholarship);
        _context.ScholarshipStudents.Add(newScholarship);

        await SaveAsync();
    }

    public async Task Update(StudentScholarshipModel model)
    {
        ScholarshipStudent entity = await _context.ScholarshipStudents
            .Include(x => x.ScholarshipStudentDocuments)
            .Include(x => x.StudentClass)
            .SingleOrDefaultAsync(x => x.Id == model.Id);

        if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? entity?.StudentClass?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentScholarshipManage))
        {
            throw new ApiException(Messages.UnauthorizedMessageError, 401);
        }

        int? institutionId = entity.InstitutionId ?? _userInfo.InstitutionID;
        short schoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(institutionId);

        if (entity != null)
        {
            entity.AmountRate = model.AmountRate;
            entity.ScholarshipAmountId = model.ScholarshipAmountId;
            entity.ScholarshipTypeId = model.ScholarshipTypeId;
            entity.Description = model.Description;
            entity.ValidFrom = model.StartingDateOfReceiving;
            entity.ValidTo = model.EndDateOfReceiving;
            entity.SchoolYear = schoolYear;
            entity.OrderNumber = model.OrderNumber;
            entity.OrderDate = model.OrderDate;
            entity.Periodicity = model.Periodicity;
            entity.FinancingOrganId = model.ScholarshipFinancingOrganId;
            entity.CommissionDate = model.CommissionDate;
            entity.CommissionNumber = model.CommissionNumber;

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
        }
    }

    public async Task Delete(int id)
    {
        var entity = await _context.ScholarshipStudents
            .Include(x => x.ScholarshipStudentDocuments)
            .Include(x => x.StudentClass)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? entity?.StudentClass?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentScholarshipManage))
        {
            throw new ApiException(Messages.UnauthorizedMessageError, 401);
        }

        if (entity != null)
        {
            _context.ScholarshipStudentDocuments.RemoveRange(entity.ScholarshipStudentDocuments);
            _context.ScholarshipStudents.Remove(entity);
            await SaveAsync();
        }
    }

    private async Task ProcessAddedDocs(StudentScholarshipModel model, ScholarshipStudent entity)
    {
        if (model.Documents == null || !model.Documents.Any() || entity == null)
        {
            return;
        }

        foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
        {
            var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
            entity.ScholarshipStudentDocuments.Add(new ScholarshipStudentDocument
            {
                Document = docModel.ToDocument(result?.Data?.BlobId)
            });
        }
    }

    private async Task ProcessDeletedDocs(StudentScholarshipModel model, ScholarshipStudent scholarship)
    {
        if (model.Documents == null || !model.Documents.Any() || scholarship == null)
        {
            return;
        }

        HashSet<int> docIdsToDelete = model.Documents
        .Where(x => x.Id.HasValue && x.Deleted == true)
        .Select(x => x.Id.Value).ToHashSet();

        await _context.ScholarshipStudentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
        await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
    }
}
}
