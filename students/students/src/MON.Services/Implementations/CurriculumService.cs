namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.StudentModels.Class;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class CurriculumService : BaseService<CurriculumService>
    {
        public CurriculumService(DbServiceDependencies<CurriculumService> dependencies)
            : base(dependencies)
        {
        }


        #region Private members
        /// <summary>
        /// Валидация на учебен план. Изпълнява се при ръчно създаване, редакция на учебен план.
        /// Проверка за:
        ///     1. Включен профилиращ предмет без включени модули към него.
        ///     2. Изключен профилиращ предмет, но включени модули към него.
        /// </summary>
        /// <param name="studentClassId"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private async Task CurriculumStudentValidityCheck(int? studentClassId)
        {
            ApiValidationResult validationResult = new ApiValidationResult();
            List<CurriculumClassModel> curriculumClassModels = await GetForStudentClass(studentClassId ?? 0, null, CancellationToken.None);

            // Включен профилиращ предмет без включени модули към него
            var includetParentWithoutIncludetModules = curriculumClassModels.Where(x => x.IsCurriculumIncluded && !x.IsModule
                && curriculumClassModels.Any(c => c.IsModule && c.ParentCurriculumId == x.CurriculumId)
                && !curriculumClassModels.Any(c => c.IsModule && c.ParentCurriculumId == x.CurriculumId && c.IsCurriculumIncluded));

            foreach (var item in includetParentWithoutIncludetModules)
            {
                validationResult.Errors.Add($"В учебния план е добавен профилиращ предмет '{item.SubjectName}', но не са добавени модули към него.");
            }

            // Изключен профилиращ предмет, но включени модули към него
            var unincludedParentWithIncludetModules = curriculumClassModels.Where(x => !x.IsCurriculumIncluded && !x.IsModule
                && curriculumClassModels.Any(c => c.IsModule && c.ParentCurriculumId == x.CurriculumId && c.IsCurriculumIncluded));

            foreach (var item in unincludedParentWithIncludetModules)
            {
                validationResult.Errors.Add($"В учебния план са добавени модули към профилиращ предмет '{item.SubjectName}', но не е добавен профилиращият предмет.");
            }

            if (validationResult.HasErrors)
            {
                throw new ApiException("Успешен запис. Валидационни грешки в учебния план.", 400, validationResult.Errors, "warn");
            }
        }

        /// <summary>
        /// Връща всички валидни(IsValid != false) CurriculumIds за даден ученик, клас и учебна година
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        
        /// <summary>
        /// Всички основни предмети ParentCurriculumId == null, които са с IsWholeClass == true.
        /// Профилиращите ще се включат само ако има модул, който е IsWholeClass == true
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        private async Task<List<int>> GetClassCurriculumIdsToEnroll(int classId, short schoolYear)
        {
            var curriculumClasses = await _context.CurriculumClasses
                .Where(x => x.ClassId == classId && x.SchoolYear == schoolYear
                    && (x.IsValid == null || x.IsValid == true))
                .Select(x => new
                {
                    x.CurriculumClassesId,
                    x.CurriculumId,
                    x.Curriculum.ParentCurriculumId,
                    x.Curriculum.IsWholeClass,
                    IsModule = x.Curriculum.ParentCurriculumId != null
                }).ToListAsync();

            List<int> ids = new List<int>();

            foreach (var cc in curriculumClasses.Where(x => !x.IsModule && x.IsWholeClass == true))
            {
                var modules = curriculumClasses.Where(x => x.IsModule && x.ParentCurriculumId == cc.CurriculumId);

                if (!modules.Any())
                {
                    // Не е профилиращ предмет и го добавяме
                    ids.Add(cc.CurriculumId);
                    continue;
                }

                var isWholeClassModules = modules.Where(x => x.IsWholeClass == true);
                if (isWholeClassModules.Any())
                {
                    // профилиращ предмет е и има модули, за който IsWholeClass == true

                    ids.Add(cc.CurriculumId); // проф. предмет
                    ids.AddRange(isWholeClassModules.Select(x => x.CurriculumId)); // модулите на проф. предмет, за които IsWholeClass == true
                }
            }

            return ids;
        }

        private async Task<List<int>> GetValidCurriculumIdsForPersonSchoolYearAndStudentClass(int personId, short schoolYear, int studentClassId)
        {
            List<int> existingCurriculums = await _context.CurriculumStudents
                .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear
                    && x.StudentId == studentClassId
                    && (x.IsValid == null || x.IsValid == true))
                .Select(x => x.CurriculumId)
                .ToListAsync();

            return existingCurriculums;
        }

        private async Task SetCurrriculumStudentsCount(List<int> curriculumIds)
        {
            if (curriculumIds ==  null || curriculumIds.Count == 0)
            {
                return;
            }

            HashSet<int> ids = curriculumIds.ToHashSet();
            List<Curriculum> curriculums = await _context.Curricula
                .Where(x => ids.Contains(x.CurriculumId))
                .ToListAsync();

            foreach (Curriculum curriculum in curriculums)
            {
                curriculum.StudentsCount = await _context.CurriculumStudents
                    .CountAsync(x => x.CurriculumId == curriculum.CurriculumId && x.IsValid == true);
            }

            await SaveAsync();
        }
        #endregion

        /// <summary>
        /// Връща учебния план на дадена паралелка/група (CurriculumClasses) за даден StudentClassId
        /// </summary>
        /// <param name="studentClassId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<CurriculumClassModel>> GetForStudentClass(int studentClassId, int? status, CancellationToken cancellationToken)
        {
            var studentClass = await _context.StudentClasses
               .Where(x => x.Id == studentClassId)
               .Select(x => new
               {
                   x.PersonId,
                   x.Class.ParentClassId
               })
               .SingleOrDefaultAsync(cancellationToken);

            if (studentClass == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(studentClass)));
            }

            if (!await _authorizationService.HasPermissionForStudent(studentClass.PersonId, DefaultPermissions.PermissionNameForStudentCurriculumRead)
                && !!await _authorizationService.HasPermissionForStudent(studentClass.PersonId, DefaultPermissions.PermissionNameForStudentCurriculumManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError);
            }

            List<CurriculumClassModel> rawModels = await (
                from cc in _context.CurriculumClasses
                join fls in _context.Fls on cc.Curriculum.FlsubjectId equals fls.Flid into temp
                from flsNom in temp.DefaultIfEmpty()
                where cc.ClassId == studentClass.ParentClassId
                let curriculum = cc.Curriculum
                select new CurriculumClassModel
                {
                    CurriculumId = cc.CurriculumId,
                    FlsubjectId = curriculum.FlsubjectId,
                    FlsubjectName = flsNom.Name,
                    IsIndividualLesson = curriculum.IsIndividualLesson,
                    SubjectId = curriculum.SubjectId,
                    SubjectName = curriculum.Subject.SubjectName,
                    SubjectTypeId = curriculum.SubjectTypeId,
                    SubjectTypeName = curriculum.SubjectType.Name,
                    CurriculumPartID = curriculum.CurriculumPartId,
                    CurriculumPartName = curriculum.CurriculumPart.Name,
                    CurriculumPartDescription = curriculum.CurriculumPart.Description,
                    IsWholeClass = curriculum.IsWholeClass,
                    IsAllStudents = curriculum.IsAllStudents,
                    CurriculumGroupNum = curriculum.CurriculumGroupNum,
                    ParentCurriculumId = curriculum.ParentCurriculumId,
                    IsSelectable = true,
                    IsModule = curriculum.ParentCurriculumId != null,
                    SortOrder = curriculum.SortOrder,
                    IsCurriculumIncluded = curriculum.CurriculumStudents.Any(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)),
                    CurriculumStudentId = curriculum.CurriculumStudents.Where(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)).Select(x => x.CurriculumStudentId).FirstOrDefault(),
                    WeeksFirstTerm = curriculum.CurriculumStudents.Where(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)).Select(x => x.WeeksFirstTerm).FirstOrDefault(),
                    HoursWeeklyFirstTerm = curriculum.CurriculumStudents.Where(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)).Select(x => x.HoursWeeklyFirstTerm).FirstOrDefault(),
                    WeeksSecondTerm = curriculum.CurriculumStudents.Where(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)).Select(x => x.WeeksSecondTerm).FirstOrDefault(),
                    HoursWeeklySecondTerm = curriculum.CurriculumStudents.Where(x => x.PersonId == studentClass.PersonId && (x.IsValid == null || x.IsValid == true)).Select(x => x.HoursWeeklySecondTerm).FirstOrDefault(),
                    CurriculumWeeksFirstTerm = curriculum.WeeksFirstTerm,
                    CurriculumHoursWeeklyFirstTerm = curriculum.HoursWeeklyFirstTerm,
                    CurriculumWeeksSecondTerm = curriculum.WeeksSecondTerm,
                    CurriculumHoursWeeklySecondTerm = curriculum.HoursWeeklySecondTerm,
                    IsValid = cc.IsValid ?? true,
                    IsIndividualCurriculum = curriculum.IsIndividualCurriculum
                })
                .ToListAsync(cancellationToken);

            List<CurriculumClassModel> curriculumClassModels = new List<CurriculumClassModel>();

            foreach (var item in rawModels.Where(x => x.ParentCurriculumId == null)
                .OrderBy(x => x.CurriculumPartID).ThenBy(x => x.SubjectId)
                .Select((x, i) => new { cc = x, index = i + 1 }))
            {
                CurriculumClassModel toAdd = item.cc;

                if (status == 0 && !item.cc.IsValid)
                {
                    continue;
                }

                if (status == 1 && item.cc.IsValid)
                {
                    continue;
                }

                toAdd.Index = item.index;
                toAdd.BC = item.index.ToString();
                toAdd.IsProfSubject = toAdd.SubjectTypeId.HasValue && GlobalConstants.SubjectTypesOfProfileSubject.Contains(toAdd.SubjectTypeId);
                curriculumClassModels.Add(toAdd);

                int index = 0;
                foreach (var module in rawModels.Where(x => x.ParentCurriculumId == toAdd.CurriculumId).OrderBy(x => x.CurriculumId).ThenBy(x => x.SubjectId))
                {
                    if (status == 0 && !module.IsValid)
                    {
                        continue;
                    }

                    if (status == 1 && module.IsValid)
                    {
                        continue;
                    }

                    module.Index = index++;
                    module.BC = $"{toAdd.Index}.{index}";
                    curriculumClassModels.Add(module);
                }
                //curriculumClassModels.AddRange(rawModels.Where(x => x.ParentCurriculumId == toAdd.CurriculumId)
                //    .OrderBy(x => x.CurriculumId)
                //    .Select((x, index) =>
                //    {
                //        x.Index = index + 1;
                //        x.BC = $"{toAdd.Index}.{index + 1}";
                //        return x;
                //    }));
            }

            return curriculumClassModels;
        }

        /// <summary>
        /// Добавя избрани CurriculumId в учебния план на даден StudentClassId.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="skipAuthorization">Пропускане на авторизацията за наличие на <seealso cref="DefaultPermissions.PermissionNameForStudentCurriculumManage"/>. Да се използва с особено внимание!</param>
        /// <param name="skipCurriculumValidation">Пропускане на валидацията на учебния план</param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<List<int>> AddSelectedForStudentClass(CurriculumStudentUpdateModel model,
            bool skipAuthorization = false,
            bool skipCurriculumValidation = true)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model)));
            }

            if (!skipAuthorization && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentCurriculumManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.CurriculumIds == null)
            {
                return null;
            }

            List<int> existingCurriculums = await GetValidCurriculumIdsForPersonSchoolYearAndStudentClass(model.PersonId, model.SchoolYear, model.StudentClassId ?? default);

            // Добавяне на всички, които не съществуват
            List<int> curriculumIdsToAdd = model.CurriculumIds.Where(x => !existingCurriculums.Contains(x)).ToList();

            if (curriculumIdsToAdd.Count == 0)
            {
                // Нищо за добавеня
                return curriculumIdsToAdd;
            }

            var curriculums = await _context.Curricula.Where(x => curriculumIdsToAdd.Contains(x.CurriculumId))
                .Select(x => new
                {
                    x.CurriculumId,
                    x.WeeksFirstTerm,
                    x.HoursWeeklyFirstTerm,
                    x.WeeksSecondTerm,
                    x.HoursWeeklySecondTerm
                })
                .ToListAsync();

            _context.CurriculumStudents.AddRange(curriculumIdsToAdd.Select(x => new CurriculumStudent
            {
                PersonId = model.PersonId,
                CurriculumId = x,
                SchoolYear = model.SchoolYear,
                StudentId = model.StudentClassId,
                SysUserId = _userInfo.SysUserID,
                IsValid = true,
                WeeksFirstTerm = curriculums.FirstOrDefault(c => c.CurriculumId == x)?.WeeksFirstTerm,
                HoursWeeklyFirstTerm = curriculums.FirstOrDefault(c => c.CurriculumId == x)?.HoursWeeklyFirstTerm,
                WeeksSecondTerm = curriculums.FirstOrDefault(c => c.CurriculumId == x)?.WeeksSecondTerm,
                HoursWeeklySecondTerm = curriculums.FirstOrDefault(c => c.CurriculumId == x)?.HoursWeeklySecondTerm,
            }));

            await SaveAsync();

            if (!skipCurriculumValidation)
            {
                await CurriculumStudentValidityCheck(model.StudentClassId);
            }
            await SetCurrriculumStudentsCount(curriculumIdsToAdd);

            return curriculumIdsToAdd;
        }

        public async Task EditCurriculumStudent(CurriculumStudentUpdateModel model)
        {
            if(model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model)));
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentCurriculumManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            CurriculumStudent curriculumStudent = await _context.CurriculumStudents.FirstOrDefaultAsync(x => x.CurriculumStudentId == model.CurriculumStudentId);
            if (curriculumStudent == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(curriculumStudent)));
            }

            curriculumStudent.WeeksFirstTerm = model.WeeksFirstTerm;
            curriculumStudent.HoursWeeklyFirstTerm = model.HoursWeeklyFirstTerm;
            curriculumStudent.WeeksSecondTerm  = model.WeeksSecondTerm;
            curriculumStudent.HoursWeeklySecondTerm = model.HoursWeeklySecondTerm;

            await SaveAsync();
        }

        /// <summary>
        /// Добавяне на учебен план за даден StudentClassId.
        /// Добавя подходящите Curriculum от учебния план на класа на StudentClass.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="stuentClassId"></param>
        /// <returns></returns>
        internal async Task<List<int>> AddForStudentClass(int personId, short schoolYear, int stuentClassId)
        {
            int? classId = await _context.StudentClasses
                .Where(x => x.Id == stuentClassId && x.PersonId == personId)
                .Select(x => x.Class.ParentClassId)
                .SingleOrDefaultAsync();

            if (!classId.HasValue)
            {
                // Todo: да се мисли дали не трябва да изгърми с грешка, че не можем да му запишем учебен план
                return null;
            }

            List<int> curriculumIds = await GetClassCurriculumIdsToEnroll(classId.Value, schoolYear);
      
            return await AddSelectedForStudentClass(new CurriculumStudentUpdateModel
            {
                PersonId = personId,
                CurriculumIds = curriculumIds.ToArray(),
                SchoolYear = schoolYear,
                StudentClassId = stuentClassId
            }, skipAuthorization: true);
        }

        /// <summary>
        /// Изтриване на учебен план за StudentClassId.
        /// При оптисване от група/паралелка следва да изтрием(сетнем IsValid = false)
        /// учебния план на ученика в старата му група/паралелка.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<List<int>> DeleteForStudentClass(CurriculumStudentUpdateModel model, bool softDelete = true)
        {
            // При отписване от институция, паралелка, местене от паралелка в паралелка,
            // записите в CurriculumStudents се маркурат с IsValid = false.
            // Пре редакция на чебния план, в този случай, се изтриват.

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model)));
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentCurriculumManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<CurriculumStudent> query = _context.CurriculumStudents
                 .Where(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear && x.StudentId == model.StudentClassId
                    && (x.IsValid == null || x.IsValid == true));

            if (model.CurriculumIds != null && model.CurriculumIds.Any())
            {
                // Изтриваме избрани.
                query = query.Where(x => model.CurriculumIds.Contains(x.CurriculumId));
            }

            List<CurriculumStudent> toDelete = await query.ToListAsync();
            if (softDelete)
            {
                foreach (var item in toDelete)
                {
                    item.IsValid = false;
                }
            } else
            {
                _context.CurriculumStudents.RemoveRange(toDelete);
            }

            await SaveAsync();

            List<int> curriculumIds = toDelete.Select(x => x.CurriculumId).ToList();
            await SetCurrriculumStudentsCount(curriculumIds);

            return curriculumIds;
        }
    }
}
