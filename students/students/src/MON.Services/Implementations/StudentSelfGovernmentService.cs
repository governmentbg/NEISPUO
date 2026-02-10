namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Vml.Office;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class StudentSelfGovernmentService : BaseService<StudentSelfGovernmentService>, IStudentSelfGovernmentService
    {
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly IStudentService _studentService;

        public StudentSelfGovernmentService(DbServiceDependencies<StudentSelfGovernmentService> dependencies,
            ILodFinalizationService lodFinalizationService,
            IStudentService studentService)
        : base(dependencies)
        {
            _lodFinalizationService = lodFinalizationService;
            _studentService = studentService;
        }

        private async Task CheckLodFinalization(SelfGovernmentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(SelfGovernmentModel)));
            }

            if(await _context.Lodfinalizations.AnyAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear && x.IsFinalized))
            {
                throw new ApiException(GlobalConstants.LodIsFinalizedError(model.SchoolYear));
            }
        }

        public async Task<SelfGovernmentModel> GetById(int id)
        {
            SelfGovernmentModel model = await _context.SelfGovernments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SelfGovernmentModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId.Value,
                    AdditionalInformation = x.AdditionalInformation,
                    ParticipationAdditionalInformation = x.ParticipationAdditionalInformation,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    InstitutionId = x.InstitutionId,
                    ParticipationId = x.ParticipationId,
                    ParticipationName = x.Participation.Name,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    Email = x.Person.Student.Email,
                    MobilePhone = x.Person.Student.MobilePhone,
                    Institution = x.InstitutionSchoolYear.Name,
                    StudentClassId= x.StudentClassId,
                }).SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSelfGovernmentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSelfGovernmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<SelfGovernmentViewModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentSelfGovernmentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.SelfGovernments
                          join lf in _context.Lodfinalizations on new { PersonId = x.PersonId ?? 0, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                        from lodFin in temp.DefaultIfEmpty()
                        where x.PersonId == personId
                        orderby x.SchoolYear descending, x.Id descending
                        select new SelfGovernmentViewModel
                        {
                            Id = x.Id,
                            PersonId = x.PersonId.Value,
                            AdditionalInformation = x.AdditionalInformation,
                            ParticipationAdditionalInformation = x.ParticipationAdditionalInformation,
                            SchoolYear = x.SchoolYear,
                            SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                            Institution = x.InstitutionSchoolYear.Abbreviation,
                            Position = x.Position.Name,
                            Participation = x.Participation.Name,
                            Email = x.Person.Student.Email,
                            MobilePhone = x.Person.Student.MobilePhone,
                            StudentClassId = x.StudentClassId,
                            IsLodFinalized = lodFin != null && lodFin.IsFinalized
                        })
                        .ToListAsync();
        }

        public async Task Create(SelfGovernmentModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSelfGovernmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError);
            }

            await CheckLodFinalization(model);

            Student student = await _context.Students.SingleOrDefaultAsync(x => x.PersonId == model.PersonId);
            if (student == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(Student)));
            }

            student.MobilePhone = model.MobilePhone;
            student.Email = model.Email;

            SelfGovernment selfGovernment = new SelfGovernment
            {
                PersonId = model.PersonId,
                AdditionalInformation = model.AdditionalInformation,
                ParticipationAdditionalInformation = model.ParticipationAdditionalInformation,
                SchoolYear = model.SchoolYear,
                InstitutionId = _userInfo.InstitutionID.Value,
                ParticipationId = model.ParticipationId,
                PositionId = model.PositionId
            };

            StudentClassViewModel mainStudentClass = await _studentService.GetMainStudentClass(model.PersonId, true, model.SchoolYear, _userInfo.InstitutionID.Value);
            if (mainStudentClass != null && mainStudentClass.IsCurrent)
            {
                selfGovernment.StudentClassId = mainStudentClass.Id;
            }

            _context.SelfGovernments.Add(selfGovernment);

            await SaveAsync();
        }

        public async Task Update(SelfGovernmentModel model)
        {
            SelfGovernment entity = await _context.SelfGovernments
                .Include(x => x.Person)
                .ThenInclude(x => x.Student)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSelfGovernmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue || !entity.InstitutionId.HasValue || _userInfo.InstitutionID.Value != entity.InstitutionId.Value)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError);
            }

            if (entity.Person == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(Person)));
            }

            if (entity.Person.Student == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(Student)));
            }

            await CheckLodFinalization(model);

            entity.AdditionalInformation = model.AdditionalInformation;
            entity.ParticipationAdditionalInformation = model.ParticipationAdditionalInformation;
            entity.SchoolYear = model.SchoolYear;
            entity.InstitutionId = model.InstitutionId;
            entity.ParticipationId = model.ParticipationId;
            entity.PositionId = model.PositionId;
            entity.Person.Student.MobilePhone = model.MobilePhone;
            entity.Person.Student.Email = model.Email;

            await SaveAsync();
        }

        public async Task Delete(int selfGovernmentId)
        {
            SelfGovernment entity = await _context.SelfGovernments.SingleOrDefaultAsync(x => x.Id == selfGovernmentId);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSelfGovernmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue || !entity.InstitutionId.HasValue || _userInfo.InstitutionID.Value != entity.InstitutionId.Value)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError);
            }

            _context.SelfGovernments.Remove(entity);
            await SaveAsync();
        }
    }
}
