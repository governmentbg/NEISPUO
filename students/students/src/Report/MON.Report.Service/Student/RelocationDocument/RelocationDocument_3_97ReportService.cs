using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Service.Student.RelocationDocument
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess.Dto;
    using MON.Report.Model;
    using MON.Services.Interfaces;
    using System;

    internal class RelocationDocument_3_97ReportService : ReportService<RelocationDocument_3_97ReportService>
    {
        public RelocationDocument_3_97ReportService(
            DbReportServiceDependencies<RelocationDocument_3_97ReportService> dependencies)
            : base(dependencies)
        {
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            int id = GetIdAsInt(parameters);

            StudentRelocationDocumentPrintModel model = _db.RelocationDocuments
                .Where(x => x.Id == id)
                .Select(x => new StudentRelocationDocumentPrintModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Pin = $"{x.Person.PersonalIdtypeNavigation.Name} {x.Person.PersonalId}",
                    StudentFullName = $"{x.Person.FirstName} {x.Person.MiddleName} {x.Person.LastName}",
                    StudentNationality = x.Person.Nationality.Name,
                    StudentBirthDate = x.Person.BirthDate,
                    StudentBirthPlaceId = x.Person.BirthPlaceTownId,
                    StudentBirthCity = x.Person.BirthPlaceTown.Name ?? x.Person.BirthPlace,
                    StudentBirthMunicipality = x.Person.BirthPlaceTown.Municipality.Name,
                    StudentBirthRegion = x.Person.BirthPlaceTown.Municipality.Region.Name,
                    StudentBirthPlaceCountry = x.Person.BirthPlaceCountryNavigation.Name,
                    CurrentStudentClassId = x.CurrentStudentClassId,
                    BasicClassId = x.CurrentStudentClass.BasicClassId,
                    BasicClassName = x.CurrentStudentClass.BasicClass.Name,
                    SchoolYearName = x.CurrentStudentClass.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    SchoolYear = x.CurrentStudentClass.SchoolYear,
                    SendingInstitutionId = x.SendingInstitutionId,
                    SendingInstitutionName = x.SendingInstitution.Name,
                    SendingInstitutionCity = x.SendingInstitution.Town.Name,
                    SendingInstitutionMunicipality = x.SendingInstitution.Town.Municipality.Name,
                    SendingInstitutionRegion = x.SendingInstitution.Town.Municipality.Region.Name,
                    SendingInstitutionDistrict = x.SendingInstitution.LocalArea.Name,
                    EduOrganization = x.CurrentStudentClass.IsHourlyOrganization != null && x.CurrentStudentClass.IsHourlyOrganization == true
                        ? "почасова"
                        : (x.CurrentStudentClass.ClassType.Name ?? "").ToLower(),
                    EduStartDate = null,
                    EduEndDate = x.DischargeDate,
                    NoteNumber = x.NoteNumber,
                    NoteDate = x.NoteDate,
                })
                .SingleOrDefault();


            model.PrincipalName = _db.DirCurrentData
                .Where(x => x.InstitutionId == model.SendingInstitutionId)
                .Select(x => x.NamesDir)
                .FirstOrDefault();

            // 19.	В удостоверението за преместване „дата от“ да е 15.09 на текущата учебна година.
            //model.EduStartDate = occupationInterval?.StartDate;
            var currentSchoolYear = _db.InstitutionSchoolYears
                   .Where(x => x.InstitutionId == model.SendingInstitutionId && x.IsCurrent)
                   .Select(x => x.SchoolYear)
               .FirstOrDefault();
            model.EduStartDate = new DateTime(currentSchoolYear, 9, 15);

            // 20.	В удостоверението за преместване  „дата до“ да е избраната в документа за преместване дата на отписване, а не датата, на която е създаден документът за преместване.
            if (!model.EduEndDate.HasValue)
            {
                FormattableString queryString = $"select * from student.fn_student_by_institution_occupation_intervals({model.PersonId})";
                IQueryable<StudentByInstitutionOccupationIntervalDto> query = _db.Set<StudentByInstitutionOccupationIntervalDto>()
                    .FromSqlInterpolated(queryString);

                StudentByInstitutionOccupationIntervalDto occupationInterval = query
                    .Where(x => x.InstitutionId == model.SendingInstitutionId)
                    .OrderByDescending(x => x.GroupNo)
                    .FirstOrDefault();

                model.EduEndDate = occupationInterval?.EndDate;
            }

            return model;
        }
    }
}
