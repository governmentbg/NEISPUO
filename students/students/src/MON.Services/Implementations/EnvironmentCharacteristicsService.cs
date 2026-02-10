namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EnvironmentCharacteristicsService : BaseService<EnvironmentCharacteristicsService>, IEnvironmentCharacteristicsService
    {
        public EnvironmentCharacteristicsService(DbServiceDependencies<EnvironmentCharacteristicsService> dependencies)
            : base(dependencies)
        {
        }

        public async Task UpdateRelativeAsync(StudentRelativeModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var relativeToUpdate = await _context.Relatives
                .FirstOrDefaultAsync(r => r.RelativeId == model.Id);

            relativeToUpdate.CurrentAddress = model.Address;
            relativeToUpdate.Description = model.Description;
            relativeToUpdate.FirstName = model.FirstName;
            relativeToUpdate.LastName = model.LastName;
            relativeToUpdate.MiddleName = model.MiddleName;
            relativeToUpdate.Notes = model.Notes;
            relativeToUpdate.WorkStatusId = model.WorkStatus?.Value;
            relativeToUpdate.RelativeTypeId = model.RelativeType.Value;
            //updating relative pin/pintype is now disabled
            //relativeToUpdate.PersonalIdtype = model.PinType?.Value;
            //relativeToUpdate.PersonalId = model.PinType?.Value == -1 ? Guid.NewGuid().ToString() : model.Pin;
            relativeToUpdate.EducationTypeId = model.EducationType?.Value;
            relativeToUpdate.Email = model.Email;
            relativeToUpdate.PhoneNumber = model.PhoneNumber;

            await SaveAsync();

            await CalculateFamilyWeights(model.PersonId);
            await SaveAsync();
            try
            {
            }
            catch
            {
                throw;
            }
        }

        public async Task AddRelativeAsync(StudentRelativeModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var student = await _context.Students.FirstOrDefaultAsync(i => i.PersonId == model.PersonId);

            if (student == null)
            {
                student = new Student()
                {
                    PersonId = model.PersonId,
                };
                _context.Students.Add(student);
            }

            student.HasParentConsent = true;

            var dbRelative = new Relative
            {
                PersonalId = model.PinType.Value == -1 ? Guid.NewGuid().ToString() : model.Pin,
                PersonalIdtype = model.PinType.Value
            };

            dbRelative.CurrentAddress = model.Address;
            dbRelative.Description = model.Description;
            dbRelative.FirstName = model.FirstName;
            dbRelative.LastName = model.LastName;
            dbRelative.MiddleName = model.MiddleName;
            dbRelative.Notes = model.Notes;
            dbRelative.WorkStatusId = model.WorkStatus?.Value;
            dbRelative.EducationTypeId = model.EducationType?.Value;
            dbRelative.Email = model.Email;
            dbRelative.PhoneNumber = model.PhoneNumber;
            dbRelative.RelativeTypeId = model.RelativeType.Value;
            dbRelative.PersonId = model.PersonId;
            _context.Relatives.Add(dbRelative);

            await SaveAsync();

            await CalculateFamilyWeights(model.PersonId);
            await SaveAsync();
        }

        public async Task UpdateEnvironmentCharacteristicsAsync(StudentEnvironmentCharacteristicsModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var student = await _context.Students.SingleOrDefaultAsync(x => x.PersonId == model.PersonId);

            if (student == null)
            {
                student = new Student()
                {
                    PersonId = model.PersonId
                };
                _context.Students.Add(student);
            }

            student.Gpname = model.GPName.Truncate(255);
            student.Gpphone = model.GPPhone.Truncate(255);
            student.HasParentConsent = model.HasParentConsent;
            student.LivesWithFosterFamily = model.LivesWithFosterFamily;
            student.RepresentedByTheMajor = model.RepresentedByTheMajor;

            int? languageId;

            if (model.NativeLanguage == null || model.NativeLanguage?.Value == 0)
            {
                languageId = null;
            }
            else
            {
                languageId = model.NativeLanguage.Value;
            }

            student.NativeLanguageId = languageId;

            await SaveAsync();
        }

        public async Task DeleteRelativeAsync(int relativeId, int personId)
        {
            if (relativeId <= 0)
            {
                throw new ApiException("RelativeId must be possitive number!");
            }

            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);

            }

            var relativeToDelete = await _context.Relatives.SingleOrDefaultAsync(x => x.RelativeId == relativeId);

            if (relativeToDelete == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            _context.Relatives.Remove(relativeToDelete);

            await SaveAsync();

            await CalculateFamilyWeights(personId);
            await SaveAsync();
        }

        public async Task<StudentRelativeModel> GetRelativeAsync(int relativeId, int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var relative = await _context.Relatives.AsNoTracking()
                .Where(x => x.RelativeId == relativeId && x.PersonId == personId)
                .Select(r => new StudentRelativeModel
                {
                    Id = r.RelativeId,
                    RelativeType = new DropdownViewModel
                    {
                        Text = r.RelativeType.Name,
                        Name = r.RelativeType.Name,
                        Value = r.RelativeType.RelativeTypeId
                    },
                    WorkStatus = new DropdownViewModel
                    {
                        Text = r.WorkStatus.Name,
                        Name = r.WorkStatus.Name,
                        Value = r.WorkStatus.WorkStatusId
                    },
                    Notes = r.Notes,
                    Address = r.CurrentAddress,
                    Description = r.Description,
                    FirstName = r.FirstName,
                    MiddleName = r.MiddleName,
                    LastName = r.LastName,
                    Pin = r.PersonalId,
                    PinType = new DropdownViewModel
                    {
                        Text = r.PersonalIdtypeNavigation.Name,
                        Name = r.PersonalIdtypeNavigation.Name,
                        Value = r.PersonalIdtypeNavigation.PersonalIdtypeId
                    },
                    Email = r.Email,
                    EducationType = new DropdownViewModel
                    {
                        Text = r.EducationType.Name,
                        Name = r.EducationType.Name,
                        Value = r.EducationType.Id
                    },
                    PhoneNumber = r.PhoneNumber
                }).SingleOrDefaultAsync();

            return relative;
        }

        public async Task<IEnumerable<StudentRelativeModel>> GetRelativesAsync(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var relatives = await _context.Relatives.AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(r => new StudentRelativeModel
                {
                    Id = r.RelativeId,
                    RelativeType = new DropdownViewModel
                    {
                        Text = r.RelativeType.Name,
                        Name = r.RelativeType.Name,
                        Value = r.RelativeType.RelativeTypeId
                    },
                    WorkStatus = new DropdownViewModel
                    {
                        Text = r.WorkStatus.Name,
                        Name = r.WorkStatus.Name,
                        Value = r.WorkStatus.WorkStatusId
                    },
                    Notes = r.Notes,
                    Address = r.CurrentAddress,
                    Description = r.Description,
                    FirstName = r.FirstName,
                    MiddleName = r.MiddleName,
                    LastName = r.LastName,
                    Pin = r.PersonalIdtype == -1 ? "" : r.PersonalId,
                    PinType = new DropdownViewModel
                    {
                        Text = r.PersonalIdtypeNavigation.Name,
                        Name = r.PersonalIdtypeNavigation.Name,
                        Value = r.PersonalIdtypeNavigation.PersonalIdtypeId
                    },
                    Email = r.Email,
                    EducationType = new DropdownViewModel
                    {
                        Text = r.EducationType.Name,
                        Name = r.EducationType.Name,
                        Value = r.EducationType.Id
                    },
                    PhoneNumber = r.PhoneNumber
                })
                .OrderBy(i => i.Id)
                .ToListAsync();

            return relatives;
        }

        public async Task<StudentEnvironmentCharacteristicsModel> GetStudentEnvironmentCharacteristics(int personId)
        {
            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.People.AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new StudentEnvironmentCharacteristicsModel
                {
                    GPName = x.Student.Gpname,
                    GPPhone = x.Student.Gpphone,
                    HasParentConsent = x.Student.HasParentConsent,
                    LivesWithFosterFamily = x.Student.LivesWithFosterFamily,
                    RepresentedByTheMajor = x.Student.RepresentedByTheMajor,
                    NativeLanguage = new DropdownViewModel
                    {
                        Text = x.Student.NativeLanguage.Name,
                        Name = x.Student.NativeLanguage.Name,
                        Value = x.Student.NativeLanguage.Id
                    },
                    FamilyEducationWeight = x.Student.FamilyEducationWeight,
                    FamilyWorkStatusWeight = x.Student.FamilyWorkStatusWeight
                })
                .SingleOrDefaultAsync();
        }

        public async Task<StudentRelativeModel> GetRelatedRelativeByPinAsync(string pin, int personId)
        {
            if (string.IsNullOrEmpty(pin))
            {
                throw new ApiException("Pin cant be empty!");
            }

            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var relatives = await _context.Relatives.AsNoTracking().Where(x => x.PersonalId == pin).Select(r => new StudentRelativeModel
            {
                Id = r.RelativeId,
                ChildId = r.ChildId ?? 0,
                RelativeType = new DropdownViewModel
                {
                    Name = r.RelativeType.Name,
                    Value = r.RelativeType.RelativeTypeId,
                    Text = r.RelativeType.Name
                },
                WorkStatus = new DropdownViewModel
                {
                    Name = r.WorkStatus.Name,
                    Value = r.WorkStatus.WorkStatusId,
                    Text = r.WorkStatus.Name,
                },
                Notes = r.Notes,
                Address = r.CurrentAddress,
                Description = r.Description,
                FirstName = r.FirstName,
                MiddleName = r.MiddleName,
                LastName = r.LastName,
                Pin = r.PersonalId,
                PinType = new DropdownViewModel
                {
                    Name = r.PersonalIdtypeNavigation.Name,
                    Value = r.PersonalIdtypeNavigation.PersonalIdtypeId,
                    Text = r.PersonalIdtypeNavigation.Name
                },
                Email = r.Email,
                EducationType = new DropdownViewModel
                {
                    Name = r.EducationType.Name,
                    Value = r.EducationType.Id,
                    Text = r.EducationType.Name
                },
                PhoneNumber = r.PhoneNumber
            }).ToListAsync();

            if (relatives.SingleOrDefault(x => x.ChildId == personId) != default)
            {
                throw new ApiException("This child already has relative with this pin.");
            }

            return relatives.FirstOrDefault(x => x.ChildId != personId);
        }

        /// <summary>
        /// Изчислява коефицентите на ученика съобразно роднините
        /// </summary>
        /// <param name="personId"></param>
        private async Task CalculateFamilyWeights(int personId)
        {
            var workStatusWeight = (await _context.Relatives.Where(x => x.PersonId == personId).AverageAsync(r => r.WorkStatus.Weight)) ?? 0.0;
            var educationWeight = (await _context.Relatives.Where(x => x.PersonId == personId).AverageAsync(r => r.EducationType.Weight)) ?? 0.0;

            Student student = await _context.Students.FirstOrDefaultAsync(i => i.PersonId == personId);
            if (student == null)
            {
                student = new Student()
                {
                    PersonId = personId
                };
            }
            student.FamilyEducationWeight = Convert.ToDecimal(educationWeight);
            student.FamilyWorkStatusWeight = Convert.ToDecimal(workStatusWeight);
        }
    }
}
