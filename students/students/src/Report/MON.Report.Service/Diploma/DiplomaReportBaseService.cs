namespace MON.Report.Service.Diploma
{
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Report.Model;
    using MON.Report.Model.Diploma;
    using MON.Shared;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Org.BouncyCastle.Crypto.Prng.Drbg;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class DiplomaReportBaseService<T> : ReportService<T>
    {
        public DiplomaReportBaseService(DbReportServiceDependencies<T> dependencies)
            : base(dependencies)
        {
        }

        private bool DemandPermissionsForDiploma(int diplomaId)
        {

            bool permission = false;
            if (diplomaId == -1)
            {
                // При стойност -1 означава dummy данни и се използва за дизайнера
                permission = true;
            }
            else
            {
                switch (_userInfo.UserRole)
                {
                    case UserRoleEnum.School:
                        var diploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                        permission = (diploma.InstitutionId == _userInfo.InstitutionID);
                        break;
                    case UserRoleEnum.Consortium:
                        permission = true;
                        break;
                    case UserRoleEnum.Ruo:
                    case UserRoleEnum.RuoExpert:
                        var dbDiploma = _db.Diplomas.FirstOrDefault(i => i.Id == diplomaId);
                        permission = (dbDiploma.RuoRegId == _userInfo.RegionID);
                        break;
                    default:
                        permission = false;
                        break;
                }
            }

            return permission;
        }

        /// <summary>
        /// Взима PartId-то от конкретната диплома за конкретната категория от оценки (по Code/Category)
        /// </summary>
        /// <param name="basicDocumentId"></param>
        /// <param name="basicDocumentPartCategory"></param>
        /// <returns></returns>
        public int? GetBasicDocumentPartCategory(int basicDocumentId, BasicDocumentPartCategoryEnum basicDocumentPartCategory)
        {
            int? basicDocumentPartCategoryInt = (int)basicDocumentPartCategory;

            int? partId = (
            from d in _db.BasicDocumentParts
            where d.BasicDocumentId == basicDocumentId && d.Code == basicDocumentPartCategoryInt.ToString()
            select d.Id).FirstOrDefault();

            return partId;
        }

        public int? GetBasicDocumentPartCategory(int basicDocumentId, BasicDocumentPartCategoryEnum basicDocumentPartCategory, int basicClassId)
        {
            int? basicDocumentPartCategoryInt = (int)basicDocumentPartCategory;

            int? partId = (
            from d in _db.BasicDocumentParts
            where d.BasicDocumentId == basicDocumentId && d.Code == basicDocumentPartCategoryInt.ToString()
                && d.BasicClassId == basicClassId
            select d.Id).FirstOrDefault();

            return partId;
        }

        public static string GetGradeTextByExternalEvaluation(List<DiplomaGradeModel> grades, int subjectId, int externalEvaluationTypeId)
        {
            string gradeText = grades?.FirstOrDefault(i => i.ExternalEvaluationTypeId == externalEvaluationTypeId 
            && i.SubjectId == subjectId).GradeText ?? null;

            return gradeText;
        }

        public override object LoadReport(IDictionary<string, object> parameters)
        {
            try
            {
                int diplomaId = GetIdAsInt(parameters);
                if (!DemandPermissionsForDiploma(diplomaId))
                {
                    // Нямаме право да отпечатаме тази диплома
                    throw new UnauthorizedAccessException("Нямате право да отпечатате тази диплома");
                }

                // Ако подадем -1, зареждаме примерни данни
                if (diplomaId == -1) return new DiplomaModel(true);

                var diploma = _db.Diplomas
                    .Where(x => x.Id == diplomaId)
                    .Select(x => new
                    {
                        x.Contents,
                        x.EduFormId,
                        Diploma = new DiplomaModel()
                        {
                            BasicDocumentId = x.BasicDocumentId,
                            BasicDocumentName = x.BasicDocument.Name,
                            Ministry = x.MinistryId != null ? x.Ministry.Name : x.MinistryName,
                            FirstName = x.FirstName,
                            MiddleName = x.MiddleName,
                            LastName = x.LastName,
                            Series = x.Series,
                            FactoryNumber = x.FactoryNumber,
                            PersonalId = x.PersonalId,
                            PersonalIdType = x.PersonalIdtype,
                            PersonId = x.PersonId,
                            RegNumberTotal = x.RegistrationNumberTotal,
                            RegNumberYear = x.RegistrationNumberYear,
                            RegDate = x.RegistrationDate != null ? x.RegistrationDate.Value.ToString("dd.MM.yyyy") : "",
                            BasicClassRomeName = x.BasicClass.RomeName,
                            BasicClassDescription = x.BasicClass.Description,
                            BasicClass = x.BasicClassId,
                            Nationality = x.NationalityNavigation.Name,
                            InstitutionId = x.InstitutionId,
                            Institution = x.InstitutionSchoolYear.Name,
                            InstitutionTownType = x.InstitutionSchoolYear.Town.Type,
                            InstitutionTown = x.InstitutionSchoolYear.Town.Name,
                            InstitutionMunicipality = x.InstitutionSchoolYear.Town.MunicipalityId != GlobalConstants.MunicipalityOther ? x.InstitutionSchoolYear.Town.Municipality.Name: null,
                            InstitutionRegion = x.InstitutionSchoolYear.Town.Municipality.RegionId != GlobalConstants.RegionOther ?  x.InstitutionSchoolYear.Town.Municipality.Region.Name : null,
                            InstitutionLocalArea = x.InstitutionSchoolYear.LocalArea.Name,
                            Description = x.Description,
                            SchoolYear = x.SchoolYear,
                            YearGraduated = x.YearGraduated,
                            ProtocolNo = x.ProtocolNumber,
                            ProtocolDate = x.ProtocolDate != null ? x.ProtocolDate.Value.ToString("dd.MM.yyyy") : "",
                            Principal = x.Principal,
                            Deputy = x.Deputy,
                            LeadTeacher = x.LeadTeacher,
                            NKR = x.Nkr,
                            EKR = x.Ekr,
                            FLLevel = x.Flgelevel.Name,
                            VetLevel = x.VetLevel,
                            VetLevelText = VetLevelConvertToText(x.VetLevel),
                            ITLevel = x.Itlevel.Description,
                            EduForm = x.EduFormName,
                            EducationType = x.EducationType.Name,
                            Gpa = x.Gpa,
                            GpaText = x.Gpatext,
                            Speciality = x.SppoospecialityName,
                            SpecialityCode = x.Sppoospeciality.SppoospecialityCode,
                            Profession = x.SppooprofessionName,
                            ProfessionCode = x.Sppooprofession.SppooprofessionCode,
                            ClassTypeId = x.ClassTypeId,
                            ClassType = (x.ClassTypeName == null || x.ClassTypeName== "") ? x.ClassType.Name : x.ClassTypeName,
                            EduDuration = x.EduDuration,
                            GraduationCommissionHead = x.GraduationCommissionMembers.OrderBy(i => i.Position).Select(i => i.FullName).FirstOrDefault(),
                            GraduationCommissionMembers = x.GraduationCommissionMembers.OrderBy(i => i.Position).Select(i => new CommissionMember() { Name = i.FullName }).ToList(),
                            CommissionOrderNumber = x.CommissionOrderNumber,
                            CommissionOrderDate = x.CommissionOrderData != null ? x.CommissionOrderData.Value.ToString("dd.MM.yyyy") : "",
                            Person = new DiplomaPersonModel()
                            {
                                Gender = x.Gender,
                                BirthDate = x.BirthDate != null ? x.BirthDate.Value.ToString("dd.MM.yyyy") : "",
                                BirthPlaceTown = x.BirthPlaceTown,
                                BirthPlaceMunicipality = x.BirthPlaceMunicipality,
                                BirthPlaceRegion = x.BirthPlaceRegion
                            },
                            DynamicData = new Dictionary<string, string>()
                        }
                    })
                    .FirstOrDefault();

                diploma.Diploma.FullInstitutionName = ((String.IsNullOrWhiteSpace(diploma.Diploma.OldInstitution) ? diploma.Diploma.Institution : diploma.Diploma.OldInstitution))
                    + " " + ((String.IsNullOrWhiteSpace(diploma.Diploma.OldInstitutionTown) ? ((diploma.Diploma.InstitutionTownType == "1" ? "гр." : "с.") + diploma.Diploma.InstitutionTown) : diploma.Diploma.OldInstitutionTown))
                    + ", общ. " + diploma.Diploma.InstitutionMunicipality
                    + (String.IsNullOrWhiteSpace(diploma.Diploma.InstitutionLocalArea) ? "" : ", район " + diploma.Diploma.InstitutionLocalArea + " ")
                    + ", обл. " + diploma.Diploma.InstitutionRegion;

                if (!string.IsNullOrEmpty(diploma?.Contents))
                {
                    JObject obj = JObject.Parse(diploma.Contents);
                    foreach (JProperty prop in obj.Properties())
                    {
                        string key = prop.Name;

                        DropdownViewModel dropdownViewModel = null;
                        try
                        {
                            dropdownViewModel = JsonConvert.DeserializeObject<DropdownViewModel>(prop.Value.ToString());
                        }
                        catch
                        {
                            // Ignore
                        }

                        if (dropdownViewModel != null)
                        {
                            diploma.Diploma.DynamicData.Add(key, dropdownViewModel.Text);
                        }
                        else
                        {
                            diploma.Diploma.DynamicData.Add(key, prop.Value.ToString());
                        }
                    }
                }

                // Ако в BirthPlace полетата има числа, това са id-та към номенклатури
                // Обработваме всяко поле поотделно, тъй като е възможно да не съответстват от Town
                if (diploma != null)
                {
                    if (Int32.TryParse(diploma.Diploma.Person.BirthPlaceTown, out int townId))
                    {
                        diploma.Diploma.Person.BirthPlaceTown = _db.Towns.Where(i => i.TownId == townId)
                            .Select(x => x.Name)
                            .FirstOrDefault();
                    }
                    if (Int32.TryParse(diploma.Diploma.Person.BirthPlaceMunicipality, out int municipalityId))
                    {
                        diploma.Diploma.Person.BirthPlaceMunicipality = _db.Municipalities
                            .Where(x => x.MunicipalityId == municipalityId)
                            .Select(x => x.Name)
                            .FirstOrDefault();
                    }
                    if (Int32.TryParse(diploma.Diploma.Person.BirthPlaceRegion, out int regionId))
                    {
                        diploma.Diploma.Person.BirthPlaceRegion = _db.Regions
                            .Where(x => x.RegionId == regionId)
                            .Select(x => x.Name)
                            .FirstOrDefault();
                    }
                }

                string graduationCommissionHead = FunctionsExtension.TryGetValue(diploma.Diploma.DynamicData, "graduationCommissionHead");
                if (!String.IsNullOrWhiteSpace(graduationCommissionHead))
                {
                    diploma.Diploma.GraduationCommissionHead = graduationCommissionHead;
                }

                var grades = _db.DiplomaSubjects
                    .Where(x => x.DiplomaId == diplomaId)
                    .Select(x => new DiplomaGradeModel
                    {
                        Id = x.Id,
                        DocumentPartId = x.BasicDocumentPartId,
                        DocumentPartCode = x.BasicDocumentPart.Code,
                        DocumentPartName = x.BasicDocumentPart.Description, // x.BasicDocumentPart.Description != null ? x.BasicDocumentPart.Description : x.BasicDocumentPart.Name,
                        DocumentPartDescription = x.BasicDocumentPart.Description,
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        Points = x.Nvopoints.HasValue ? Convert.ToDecimal(string.Format("{0:0.00}", x.Nvopoints.Value)) : x.Nvopoints.GetValueOrDefault(),
                        ExternalEvaluationTypeList = x.BasicDocumentPart.ExternalEvaluationTypesList,
                        BasicClass = x.BasicDocumentPart.BasicClass,
                        BasicClassId = x.BasicDocumentPart.BasicClassId,
                        BasicSubjectTypeAbrev = x.BasicDocumentPart.BasicSubjectType.Abrev,
                        BasicSubjectType = x.BasicDocumentPart.BasicSubjectTypeId,
                        SubjectName = string.IsNullOrWhiteSpace(x.SubjectName) ? x.Subject.SubjectName : x.SubjectName,
                        SubjectNameShort = x.Subject.SubjectNameShort,
                        GradeCategory = x.GradeCategory,
                        SpecialNeedsGrade = x.SpecialNeedsGrade,
                        OtherGrade = x.OtherGrade,
                        QualitativeGrade = x.QualitativeGrade,
                        Grade = x.Grade,
                        GradeText = x.GradeText,
                        ECTS = x.Ects,
                        Horarium = x.Horarium,
                        Position = x.Position,
                        FLLevel = x.FlLevel,
                        ParentId = x.ParentId,
                        ParentPosition = x.Parent.Position,
                        FLSubjectId = x.FlSubjectId,
                        FLSubjectName = x.FlSubject.Name,
                        FLHorarium = x.FlHorarium
                    })
                    .OrderBy(i => i.DocumentPartId).ThenBy(i => i.Position)
                    .ToList();

                if (!grades.IsNullOrEmpty())
                {
                    Dictionary<int, string> gradeNomenclature = (_db.GradeNoms.Select(x => new { x.Id, x.Description })
                        .ToArray())
                        .ToDictionary(x => x.Id, x => x.Description);
                    foreach (var grade in grades)
                    {
                        grade.ExternalEvaluationTypeId = grade.ExternalEvaluationTypeList != null ? (grade.ExternalEvaluationTypeList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .FirstOrDefault() : (int?)null;

                        grade.TextGradeFixture(gradeNomenclature, diploma.Diploma.BasicDocumentName);

                        if (grade.FLSubjectId != null)
                        {
                            // Предметът е изучаван на чужд език. попълваме допълнителното поле
                            grade.FLAddition = $"(на {grade.FLSubjectName} - {grade.FLHorarium} часа)";
                            grade.SubjectName = $"{grade.SubjectName} {grade.FLAddition}";
                        }
                    }
                }


                // Подреждане спрямо модулите
                diploma.Diploma.Grades = grades.OrderBy(i => i.DocumentPartId).ThenBy(i => i.ParentId == null ? 10000 + i.Position * 100 : 10000 + i.ParentPosition * 100 + i.Position).ToList();

                diploma.Diploma.GradesByBasicDocumentPartCode = grades
                    .GroupBy(x => x.DocumentPartCode)
                    .ToDictionary(x => x.Key, x => x.OrderBy(g => g.Position).ToList());

                if (diploma.EduFormId.HasValue) {
                    var eduForm = _db.EduForms.FirstOrDefault(i => i.ClassEduFormId == diploma.EduFormId);

                    if (eduForm != null)
                    {
                        diploma.Diploma.EduForm = eduForm.NameShort;
                    }
                }

                return diploma.Diploma;
            }
            catch (Exception ex)
            {
                _logger.LogError("Diploma generation report", ex);
                throw;
            }
        }

        protected DiplomaDocumentOriginal GetAdditionalDocument(int diplomaId)
        {
            var additionalDocument = (
                from o in _db.DiplomaAdditionalDocuments
                let inst = _db.Institutions.FirstOrDefault(i => i.InstitutionId == o.InstitutionId)
                where o.DiplomaId == diplomaId
                select new DiplomaDocumentOriginal()
                {
                    FactoryNumber = o.FactoryNumber,
                    Series = o.Series,
                    RegDate = o.RegistrationDate != null ? o.RegistrationDate.Value.ToString("dd.MM.yyyy") : "",
                    RegNumberTotal = o.RegistratioNumber,
                    RegNumberYear = o.RegistrationNumberYear,
                    Institution = o.InstitutionName != null ? o.InstitutionName : (o.InstitutionId != null ? _db.Institutions.FirstOrDefault(i => i.InstitutionId == o.InstitutionId).Name : ""),
                    InstitutionTown = !String.IsNullOrWhiteSpace(o.Town) ? o.Town : inst.Town.Name,
                    InstitutionLocalArea = !String.IsNullOrWhiteSpace(o.LocalArea) ? o.LocalArea : inst.LocalArea.Name,
                    InstitutionMunicipality = !String.IsNullOrWhiteSpace(o.Municipality) ? o.Municipality : (inst.Town.MunicipalityId != GlobalConstants.MunicipalityOther ? inst.Town.Municipality.Name : null),
                    InstitutionRegion = !String.IsNullOrWhiteSpace(o.Region) ? o.Region : (inst.Town.Municipality.RegionId != GlobalConstants.RegionOther ? inst.Town.Municipality.Region.Name : null),
                    InstitutionAddress = !String.IsNullOrWhiteSpace(o.InstitutionAddress) ? o.InstitutionAddress : (inst != null ? @$"гр/с {inst.Town.Name}, общ. {inst.Town.Municipality.Name}, район {inst.LocalArea.Name}, обл. {inst.Town.Municipality.Region.Name} " : ""),
                    BasicDocumentId = o.BasicDocumentId,
                    BasicDocument = o.BasicDocument.Name,
                    IsValidation = o.BasicDocument.IsValidation
                }).FirstOrDefault();

            return additionalDocument;
        }

        protected DiplomaDocumentOriginal GetAdditionalDocumentByType(int diplomaId, List<int?> basicDocumentIds)
        {
            var additionalDocument = (
                from o in _db.DiplomaAdditionalDocuments
                let inst = _db.Institutions.FirstOrDefault(i => i.InstitutionId == o.InstitutionId)
                where o.DiplomaId == diplomaId && basicDocumentIds.Contains(o.BasicDocumentId)
                select new DiplomaDocumentOriginal()
                {
                    FactoryNumber = o.FactoryNumber,
                    Series = o.Series,
                    RegDate = o.RegistrationDate != null ? o.RegistrationDate.Value.ToString("dd.MM.yyyy") : "",
                    RegNumberTotal = o.RegistratioNumber,
                    RegNumberYear = o.RegistrationNumberYear,
                    Institution = o.InstitutionName != null ? o.InstitutionName : (o.InstitutionId != null ? _db.Institutions.FirstOrDefault(i => i.InstitutionId == o.InstitutionId).Name : ""),
                    InstitutionTown = !String.IsNullOrWhiteSpace(o.Town) ? o.Town : inst.Town.Name,
                    InstitutionLocalArea = !String.IsNullOrWhiteSpace(o.LocalArea) ? o.LocalArea : inst.LocalArea.Name,
                    InstitutionMunicipality = !String.IsNullOrWhiteSpace(o.Municipality) ? o.Municipality : (inst.Town.MunicipalityId != GlobalConstants.MunicipalityOther ? inst.Town.Municipality.Name : null),
                    InstitutionRegion = !String.IsNullOrWhiteSpace(o.Region) ? o.Region : (inst.Town.Municipality.RegionId != GlobalConstants.RegionOther ? inst.Town.Municipality.Region.Name : null),
                    InstitutionAddress = !String.IsNullOrWhiteSpace(o.InstitutionAddress) ? o.InstitutionAddress : (inst != null ? @$"гр/с {inst.Town.Name}, общ. {inst.Town.Municipality.Name}, район {inst.LocalArea.Name}, обл. {inst.Town.Municipality.Region.Name} " : ""),
                    BasicDocumentId = o.BasicDocumentId,
                    BasicDocument = o.BasicDocument.Name,
                    IsValidation = o.BasicDocument.IsValidation,
                    GraduationType = o.BasicDocument.IsValidation ? "валидиране на" : "завършен"
                }).FirstOrDefault();

            return additionalDocument;
        }


        /// <summary>
        /// Оценки от чужди езици
        /// </summary>
        /// <param name="diploma"></param>
        protected void FillFLGrades(DiplomaModel diploma)
        {
            var foreignLanguageIds = _db.Fls.Where(i => i.Flid > 0).Select(i => i.Flid).ToList();

            diploma.FLGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.MandatoryBasicDocumentPartId
                && i.SubjectId != null
                && foreignLanguageIds.Contains(i.SubjectId.Value)).ToList();
        }

        /// <summary>
        /// Оценки от ЗУЧ секции и всички останали
        /// </summary>
        /// <param name="diploma"></param>
        protected void FillMandatoryAndNonMandatoryGrades(DiplomaModel diploma)
        {
            diploma.MandatoryGrades = diploma.Grades.Where(i => i.DocumentPartId == diploma.MandatoryBasicDocumentPartId).ToList();
            diploma.NonMandatoryGrades = diploma.Grades.Where(i => i.DocumentPartId != diploma.MandatoryBasicDocumentPartId).ToList();
        }

        /// <summary>
        /// Национално външно оценяване - преобразуване на точки в оценка
        /// (x + 68)/28
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>

        private static decimal NeePointsToGrade(decimal points)
        {
            if (points < 1) return 2;

            decimal grade = (points + 68) / 25m;
            return decimal.Round(grade, 3, System.MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Степен на професионална квалификация - преобразуване на число в думи
        /// </summary>
        /// <param name="vetLevel"></param>
        /// <returns></returns>
        private static string VetLevelConvertToText(int? vetLevel)
        {
            return vetLevel switch
            {
                1 => VetLevelEnum.One.GetEnumDescriptionAttrValue(),
                2 => VetLevelEnum.Two.GetEnumDescriptionAttrValue(),
                3 => VetLevelEnum.Three.GetEnumDescriptionAttrValue(),
                4 => VetLevelEnum.Four.GetEnumDescriptionAttrValue(),
                _ => "",
            };
        }

        /// <summary>
        /// Държавен зрелостен изпит - преобразуване на точки в оценка
        /// (x + 68)/28
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static decimal SmePointsToGrade(decimal points)
        {
            // Данните са взети от: https://www.otlichnik.bg/%d0%b4%d1%8a%d1%80%d0%b6%d0%b0%d0%b2%d0%b5%d0%bd-%d0%b7%d1%80%d0%b5%d0%bb%d0%be%d1%81%d1%82%d0%b5%d0%bd-%d0%b8%d0%b7%d0%bf%d0%b8%d1%82-%d1%81%d0%ba%d0%b0%d0%bb%d0%b0/
            Dictionary<decimal, decimal> grades = new Dictionary<decimal, decimal>
            {
                { 23m, 3.00m },
                { 23.5m, 3.014m },
                { 24m, 3.028m },
                { 24.5m, 3.042m },
                { 25m, 3.056m },
                { 25.5m, 3.069m },
                { 26m, 3.083m },
                { 26.5m, 3.097m },
                { 27m, 3.111m },
                { 27.5m, 3.125m },
                { 28m, 3.139m },
                { 28.5m, 3.153m },
                { 29m, 3.167m },
                { 29.5m, 3.181m },
                { 30m, 3.194m },
                { 30.5m, 3.208m },
                { 31m, 3.222m },
                { 31.5m, 3.236m },
                { 32m, 3.250m },
                { 32.5m,3.264m },
                { 33m, 3.278m },
                { 33.5m, 3.292m },
                { 34m, 3.306m },
                { 34.5m, 3.319m },
                { 35m, 3.333m },
                { 35.5m, 3.347m },
                { 36m, 3.361m },
                { 36.5m, 3.375m },
                { 37m, 3.389m },
                { 37.5m, 3.403m },
                { 38m, 3.417m },
                { 38.5m, 3.431m },
                { 39m, 3.444m },
                { 39.5m, 3.458m },
                { 40m, 3.472m },
                { 40.5m, 3.486m },
                { 41m, 3.500m },
                { 41.5m, 3.528m },
                { 42m, 3.556m },
                { 42.5m, 3.583m },
                { 43m, 3.611m },
                { 43.5m, 3.639m },
                { 44m, 3.667m },
                { 44.5m, 3.694m },
                { 45m, 3.722m },
                { 45.5m, 3.750m },
                { 46m, 3.778m },
                { 46.5m, 3.806m },
                { 47m, 3.833m },
                { 47.5m, 3.861m },
                { 48m, 3.889m },
                { 48.5m, 3.917m },
                { 49m, 3.944m },
                { 49.5m, 3.972m },
                { 50m, 4.000m },
                { 50.5m, 4.028m },
                { 51m, 4.056m },
                { 51.5m, 4.083m },
                { 52m, 4.111m },
                { 52.5m, 4.139m },
                { 53m, 4.167m },
                { 53.5m, 4.194m },
                { 54m, 4.222m },
                { 54.5m, 4.259m },
                { 55m, 4.278m },
                { 55.5m, 4.306m },
                { 56m, 4.333m },
                { 56.5m, 4.361m },
                { 57m, 4.389m },
                { 57.5m, 4.717m },
                { 58m, 4.444m },
                { 58.5m, 4.472m },
                { 59m, 4.500m },
                { 59.5m, 4.528m },
                { 60m, 4.556m },
                { 60.5m, 4.583m },
                { 61m, 4.611m },
                { 61.5m, 4.639m },
                { 62m, 4.667m },
                { 62.5m, 4.694m },
                { 63m, 4.722m },
                { 63.5m, 4.750m },
                { 64m, 4.778m },
                { 64.5m, 4.806m },
                { 65m, 4.833m },
                { 65.5m, 4.861m },
                { 66m, 4.889m },
                { 66.5m, 4.917m },
                { 67m, 4.944m },
                { 67.5m, 4.972m },
                { 68m, 5.000m },
                { 68.5m, 5.028m },
                { 69m, 5.056m },
                { 69.5m, 5.083m },
                { 70m, 5.111m },
                { 70.5m, 5.139m },
                { 71m, 5.167m },
                { 71.5m, 5.194m },
                { 72m, 5.222m },
                { 72.5m, 5.250m },
                { 73m, 5.278m },
                { 73.5m, 5.306m },
                { 74m, 5.333m },
                { 74.5m, 5.361m },
                { 75m, 5.389m },
                { 75.5m, 5.417m },
                { 76m, 5.444m },
                { 76.5m, 5.472m },
                { 77m, 5.500m },
                { 77.5m, 5.514m },
                { 78m, 5.528m },
                { 78.5m, 5.542m },
                { 79m, 5.556m },
                { 79.5m, 5.569m },
                { 80m, 5.583m },
                { 80.5m, 5.597m },
                { 81m, 5.611m },
                { 81.5m, 5.625m },
                { 82m, 5.639m },
                { 82.5m, 5.653m },
                { 83m, 5.667m },
                { 83.5m, 5.681m },
                { 84m, 5.694m },
                { 84.5m, 5.708m },
                { 85m, 5.722m },
                { 85.5m, 5.736m },
                { 86m, 5.750m },
                { 86.5m, 5.764m },
                { 87m, 5.778m },
                { 87.5m, 5.792m },
                { 88m, 5.806m },
                { 88.5m, 5.819m },
                { 89m, 5.833m },
                { 89.5m, 5.847m },
                { 90m, 5.861m },
                { 90.5m, 5.875m },
                { 91m, 5.889m },
                { 91.5m, 5.903m },
                { 92m, 5.917m },
                { 92.5m, 5.931m },
                { 93m, 5.944m },
                { 93.5m, 5.958m },
                { 94m, 5.972m },
                { 94.5m, 5.986m },
                { 95.5m, 6.000m },
                { 96m, 6.000m },
                { 96.5m, 6.000m },
                { 97m, 6.000m },
                { 97.5m, 6.000m },
                { 98m, 6.000m },
                { 98.5m, 6.000m },
                { 99m, 6.000m },
                { 99.5m, 6.000m },
                { 100m, 6.000m },
            };

            grades.TryGetValue(points, out decimal grade);

            return grade;
        }

    }
}
