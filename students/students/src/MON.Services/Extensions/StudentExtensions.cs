using MON.DataAccess;
using MON.DataAccess.Dto;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Search;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Linq;

namespace MON.Services
{
    public static class StudentExtensions
    {
        /// <summary>
        /// Филтрира по Id на институция свързана към EducationalStates ако логнатия потребител е с роля School.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static IQueryable<Person> FilterByInstitution(this IQueryable<Person> query, IUserInfo userInfo)
        {
            if (userInfo == null) return query;

            int userInstitutionId = userInfo.InstitutionID ?? int.MinValue;

            return query.Where(userInfo.SysRoleID == (int)UserRoleEnum.School,
                predicate => predicate.EducationalStates.Any(y => y.InstitutionId == userInstitutionId)
                    // TODO - Временно са добавени и тези, които имат поне един документ за преместване, Който не е използван
                    || (predicate.RelocationDocuments.Any(i => !i.AdmissionDocuments.Any()))
                    // Има паралелка, която е enrolled в нашата институция
                    || (predicate.StudentClasses.Any(i => i.Status == 1 && i.Class.InstitutionId == userInstitutionId))
                    // Дали е даден достъп до ученика през "Други институции"
                    || predicate.OtherInstitutions.Any(y => y.InstitutionId == userInstitutionId)
                    // Дали в момента няма активен StudentClass в позиция 3 и трябва да излиза
                    || !predicate.StudentClasses.Any(i => i.IsCurrent && i.PositionId == 3)
                    // Дали е в текуща паралелка на тази институция
                    //|| predicate.StudentClasses.Any(i => i.IsCurrent && i.PositionId == 3  && i.Status == 1 && i.Class.InstitutionId == userInstitutionId)
                    );
        }

        /// <summary>
        /// Филтрира по Ид на институция свързана към EducationalStates ако логнатия потребител е с роля School.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static IQueryable<StudentDto> FilterByInstitution(this IQueryable<StudentDto> query, IUserInfo userInfo)
        {
            if (userInfo == null) return query;

            int userInstitutionId = userInfo.InstitutionID ?? int.MinValue;

            // TODO - Временно са добавени и тези, които имат поне един документ за преместване
            return query.Where(userInfo.SysRoleID == (int)UserRoleEnum.School,
                predicate => predicate.InstitutionId == userInstitutionId || predicate.HasRelocationDocuments == true);
        }

        /// <summary>
        /// Филтрира по Ид на Регион на институция свързана към EducationalStates ако логнатия потребител е с роля Ruo.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static IQueryable<Person> FilterByRegion(this IQueryable<Person> query, IUserInfo userInfo)
        {
            if (userInfo == null) return query;

            int userRegionId = userInfo.RegionID ?? int.MinValue;

            return query.Where(userInfo.SysRoleID == (int)UserRoleEnum.Ruo,
                predicate => predicate.EducationalStates
                    .Any(y => y.Institution.Town.Municipality.RegionId == userRegionId));
        }

        /// <summary>
        /// Филтрира по Ид на Регион на институция свързана към EducationalStates ако логнатия потребител е с роля Ruo.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static IQueryable<StudentDto> FilterByRegion(this IQueryable<StudentDto> query, IUserInfo userInfo)
        {
            if (userInfo == null) return query;

            int userRegionId = userInfo.RegionID ?? int.MinValue;

            return query.Where(userInfo.SysRoleID == (int)UserRoleEnum.Ruo,
                predicate => predicate.InstitutionRegionId == userRegionId);
        }

        public static IQueryable<Person> FilterBySearchModel(this IQueryable<Person> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            return studentSearchModel.ExactMatch
                ? query.FilterByExactMatch(studentSearchModel, userInfo)
                : query.FilterByContains(studentSearchModel, userInfo);
        }
        public static IQueryable<StudentSearchDto> FilterBySearchModel(this IQueryable<StudentSearchDto> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            return studentSearchModel.ExactMatch
                ? query.FilterByExactMatch(studentSearchModel, userInfo)
                : query.FilterByContains(studentSearchModel, userInfo);
        }

        public static IQueryable<Person> FilterByFilterString(this IQueryable<Person> query, string filterString, IUserInfo userInfo)
        {
            // Филтрииране чрез текстово поле Търси
            if (userInfo?.IsSchoolDirector ?? false)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим в училище, област и община
                return query
                    .Where(!filterString.IsNullOrWhiteSpace(),
                       predicate => predicate.PersonalId.Contains(filterString)
                       || predicate.FirstName.Contains(filterString)
                       || predicate.MiddleName.Contains(filterString)
                       || predicate.LastName.Contains(filterString));
            }

            if (userInfo?.IsRuo ?? false)
            {
                // Вече сме филтрирали по област и не е нужно да търсим в област
                return query
                    .Where(!filterString.IsNullOrWhiteSpace(),
                        predicate => predicate.PersonalId.Contains(filterString)
                        || predicate.FirstName.Contains(filterString)
                        || predicate.MiddleName.Contains(filterString)
                        || predicate.LastName.Contains(filterString)
                        || predicate.CurrentTown.Municipality.Name.Contains(filterString)
                        || predicate.EducationalStates.Any(x => x.Institution.Name.Contains(filterString)));
            }

            return query
                .Where(!filterString.IsNullOrWhiteSpace(),
                    predicate => predicate.PersonalId.Contains(filterString)
                    || predicate.FirstName.Contains(filterString)
                    || predicate.MiddleName.Contains(filterString)
                    || predicate.LastName.Contains(filterString)
                    || predicate.CurrentTown.Municipality.Region.Name.Contains(filterString)
                    || predicate.CurrentTown.Municipality.Name.Contains(filterString)
                    || predicate.EducationalStates.Any(x => x.Institution.Name.Contains(filterString)));
        }

        public static IQueryable<StudentSearchDto> FilterByFilterString(this IQueryable<StudentSearchDto> query, string filterString, IUserInfo userInfo)
        {
            // Филтрииране чрез текстово поле Търси
            if (userInfo?.IsSchoolDirector ?? false)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим в училище, област и община
                return query
                    .Where(!filterString.IsNullOrWhiteSpace(),
                       predicate => predicate.Pin.Contains(filterString)
                       || predicate.FirstName.Contains(filterString)
                       || predicate.MiddleName.Contains(filterString)
                       || predicate.LastName.Contains(filterString));
            }

            if (userInfo?.IsRuo ?? false)
            {
                // Вече сме филтрирали по област и не е нужно да търсим в област
                return query
                    .Where(!filterString.IsNullOrWhiteSpace(),
                        predicate => predicate.Pin.Contains(filterString)
                        || predicate.FirstName.Contains(filterString)
                        || predicate.MiddleName.Contains(filterString)
                        || predicate.LastName.Contains(filterString)
                        || predicate.Municipality.Contains(filterString)
                        || predicate.School.Contains(filterString));
            }

            return query
                .Where(!filterString.IsNullOrWhiteSpace(),
                    predicate => predicate.Pin.Contains(filterString)
                    || predicate.FirstName.Contains(filterString)
                    || predicate.MiddleName.Contains(filterString)
                    || predicate.LastName.Contains(filterString)
                    || predicate.District.Contains(filterString)
                    || predicate.Municipality.Contains(filterString)
                    || predicate.School.Contains(filterString));
        }

        public static int CurrentSchoolYear(this DateTime today)
        {
            return today.Month >= 9 && today.Day >= 1
                ? today.Year
                : today.Year - 1;
        }

        private static IQueryable<Person> FilterByExactMatch(this IQueryable<Person> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            if (userInfo?.SysRoleID == (int)UserRoleEnum.School)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим училище, област и община
                return query = query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId == studentSearchModel.Pin)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber));
            }

            if (userInfo?.SysRoleID == (int)UserRoleEnum.Ruo)
            {
                // Вече сме филтрирали по област и не е нужно да търсим област
                return query = query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId == studentSearchModel.Pin)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber)
                   && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || predicate.CurrentTown.Municipality.Name == studentSearchModel.Municipality)
                   && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.EducationalStates.OrderByDescending(o => o.EducationalStateId).FirstOrDefault().Institution.Name == studentSearchModel.School));
            }

            return query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId == studentSearchModel.Pin)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber)
                   && (studentSearchModel.District.IsNullOrWhiteSpace() || predicate.CurrentTown.Municipality.Region.Name == studentSearchModel.District)
                   && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || predicate.CurrentTown.Municipality.Name == studentSearchModel.Municipality)
                   && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.EducationalStates.OrderByDescending(o => o.EducationalStateId).FirstOrDefault().Institution.Name == studentSearchModel.School));
        }

        private static IQueryable<StudentSearchDto> FilterByExactMatch(this IQueryable<StudentSearchDto> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            if (userInfo?.SysRoleID == (int)UserRoleEnum.School)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим училище, област и община
                return query = query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin == studentSearchModel.Pin)
                   && (studentSearchModel.PinType == null || predicate.PinType == studentSearchModel.PinType)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber));
            }

            if (userInfo?.SysRoleID == (int)UserRoleEnum.Ruo)
            {
                // Вече сме филтрирали по област и не е нужно да търсим област
                return query = query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin == studentSearchModel.Pin)
                   && (studentSearchModel.PinType == null || predicate.PinType == studentSearchModel.PinType)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber)
                   && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || predicate.Municipality == studentSearchModel.Municipality)
                   && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.School == studentSearchModel.School));
            }

            return query.Where(true,
                   predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin == studentSearchModel.Pin)
                   && (studentSearchModel.PinType == null || predicate.PinType == studentSearchModel.PinType)
                   && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName == studentSearchModel.FirstName)
                   && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName == studentSearchModel.MiddleName)
                   && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName == studentSearchModel.LastName)
                   && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber == studentSearchModel.PublicEduNumber)
                   && (studentSearchModel.District.IsNullOrWhiteSpace() || predicate.District == studentSearchModel.District)
                   && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || predicate.Municipality == studentSearchModel.Municipality)
                   && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.School == studentSearchModel.School));
        }

        private static IQueryable<Person> FilterByContains(this IQueryable<Person> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            if (userInfo?.SysRoleID == (int)UserRoleEnum.School)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим в училище, област и община
                return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber)));
            }

            if (userInfo?.SysRoleID == (int)UserRoleEnum.Ruo)
            {
                // Вече сме филтрирали по област и не е нужно да търсим в област
                return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber))
                    && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || (predicate.CurrentTown.Municipality.Name ?? "").Contains(studentSearchModel.Municipality))
                    && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.EducationalStates.OrderByDescending(o => o.EducationalStateId).FirstOrDefault().Institution.Name.Contains(studentSearchModel.School)));
            }

            return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.PersonalId.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber))
                    && (studentSearchModel.District.IsNullOrWhiteSpace() || (predicate.CurrentTown.Municipality.Region.Name ?? "").Contains(studentSearchModel.District))
                    && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || (predicate.CurrentTown.Municipality.Name ?? "").Contains(studentSearchModel.Municipality))
                    && (studentSearchModel.School.IsNullOrWhiteSpace() || predicate.EducationalStates.OrderByDescending(o => o.EducationalStateId).FirstOrDefault().Institution.Name.Contains(studentSearchModel.School)));
        }

        private static IQueryable<StudentSearchDto> FilterByContains(this IQueryable<StudentSearchDto> query, StudentSearchModelExtended studentSearchModel, IUserInfo userInfo)
        {
            if (userInfo?.SysRoleID == (int)UserRoleEnum.School)
            {
                // Вече сме филтрирали по училище и не е нужно да турсим в училище, област и община
                return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber)));
            }

            if (userInfo?.SysRoleID == (int)UserRoleEnum.Ruo)
            {
                // Вече сме филтрирали по област и не е нужно да търсим в област
                return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber))
                    && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || (predicate.Municipality ?? "").Contains(studentSearchModel.Municipality))
                    && (studentSearchModel.School.IsNullOrWhiteSpace() || (predicate.School ?? "").Contains(studentSearchModel.School)));
            }

            return query.Where(true,
                    predicate => (studentSearchModel.Pin.IsNullOrWhiteSpace() || predicate.Pin.Contains(studentSearchModel.Pin))
                    && (studentSearchModel.FirstName.IsNullOrWhiteSpace() || predicate.FirstName.Contains(studentSearchModel.FirstName))
                    && (studentSearchModel.MiddleName.IsNullOrWhiteSpace() || predicate.MiddleName.Contains(studentSearchModel.MiddleName))
                    && (studentSearchModel.LastName.IsNullOrWhiteSpace() || predicate.LastName.Contains(studentSearchModel.LastName))
                    && (studentSearchModel.PublicEduNumber.IsNullOrWhiteSpace() || predicate.PublicEduNumber.Contains(studentSearchModel.PublicEduNumber))
                    && (studentSearchModel.District.IsNullOrWhiteSpace() || (predicate.District ?? "").Contains(studentSearchModel.District))
                    && (studentSearchModel.Municipality.IsNullOrWhiteSpace() || (predicate.Municipality ?? "").Contains(studentSearchModel.Municipality))
                    && (studentSearchModel.School.IsNullOrWhiteSpace() || (predicate.School ?? "").Contains(studentSearchModel.School)));
        }
    }
}
