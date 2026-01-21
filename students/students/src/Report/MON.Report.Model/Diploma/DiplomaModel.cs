using MON.Report.Model.Diploma;
using System;
using System.Collections.Generic;

namespace MON.Report.Model
{
    public class DiplomaModel : InstitutionModel
    {
        public DiplomaModel()
        {

        }

        /// <summary>
        /// Използва се за попълване на примерни данни при използване на дизайнера
        /// </summary>
        /// <param name="dummyData"></param>
        public DiplomaModel(bool dummyData)
        {
            if (dummyData)
            {
                Ministry = "Министерство на образованието и науката";
                Institution = "Професионална гимназия по механизация на селското стопанство \"Пейо Крачолов Яворов\"";
                OldInstitution = "Професионална гимназия по селското стопанство \"Пейо Крачолов Яворов\"";
                InstitutionTownType = "2";
                InstitutionTown = "Негован";
                OldInstitutionTown = "Стар Негован";
                InstitutionMunicipality = "Столична";
                InstitutionRegion = "София-град";
                InstitutionLocalArea = "р-н Искър";
                Description = "Известен факт е, че читателя обръща внимание на съдържанието, което чете, а не на оформлението му. Свойството на Lorem Ipsum е, че до голяма степен има нормално разпределение на буквите и се чете по-лесно, за разлика от нормален текст на английски език като \"Това е съдържание, това е съдържание\". Много системи за публикуване и редактори на Уеб страници използват Lorem Ipsum като примерен текстов модел \"по подразбиране\", поради което при търсене на фразата \"lorem ipsum\" в Интернет ще бъдат открити много сайтове в процес на разработка. Някой от тези сайтове биват променяни с времето, а други по случайност или нарочно(за забавление и пр.) биват оставяни в този си незавършен вид.";
                PersonalIdType = 0;
                PersonalId = "1234567890";
                BasicClass = 9;
                BasicClassRomeName = "IX";
                BasicClassDescription = "девети клас";
                FirstName = "Иван";
                MiddleName = "Петров";
                LastName = "Стефанов";
                Series = "ЮА";
                FactoryNumber = "123456";
                RegNumberTotal = "2192";
                RegNumberYear = "123";
                RegDate = DateTime.Now.ToString("dd.MM.yyyy");
                SchoolYear = DateTime.Now.Year;
                YearGraduated = DateTime.Now.AddYears(1).Year;
                NKR = 3;
                EKR = 3;
                ProtocolNo = "143";
                ProtocolDate = "07.09.2022";
                EduForm = "дневна";
                EduDuration = 4;
                Gpa = 5.37m;
                GpaText = "Много добър";
                Principal = "Костадин Костадинов-Владов";
                Deputy = "Стефан Иванов";
                LeadTeacher = "Анатоли Иванов";
                Nationality = "България";
                EducationType = "начален етап";
                ITLevel = "няма";
                FLLevel = "B2";
                VetLevel = 3;
                VetLevelText = "трета";
                Speciality = "Живопис със специалност по конретизация на природното изкуство и илюстриране на исторически артефакти";
                SpecialityCode = "123456";
                Profession = "Художник с профил по калиграфично тънко изкуствознание на кирилица приложимо в публичното изкуство";
                ProfessionCode = "654321";
                ClassType = "Профил: чужди езици";
                MandatoryBasicDocumentPartId = 14;
                GraduationCommissionHead = "Андрей Андреев Андреев";
                CommissionOrderNumber = "123456";
                CommissionOrderDate = "09.11.2022";
                Person = new DiplomaPersonModel()
                {
                    Gender = 1,
                    BirthDate = "18.12.2004",
                    BirthPlaceTown = "Негован",
                    BirthPlaceMunicipality = "Столична",
                    BirthPlaceRegion = "София-град"
                };
                Grades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        DocumentPartId = 14,
                        SubjectId = 1,
                        SubjectName = "Български език и литература",
                        GradeText = "Много добър",
                        Grade = 5.34m,
                        Horarium = 1125
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        DocumentPartId = 14,
                        SubjectId = null,
                        GradeCategory = -1,
                        SubjectName = "ОБчч"
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        DocumentPartId = 14,
                        SubjectId = 17,
                        SubjectName = "Гражданско образование",
                        GradeText = "Отличен",
                        Grade = 5.95m,
                        Horarium = 235
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        DocumentPartId = 14,
                        SubjectId = 2,
                        SubjectName = "Математика",
                        GradeText = "Отличен",
                        Grade = 5.93m,
                        Horarium = 135,
                        FLSubjectId = 101,
                        FLSubjectName = "Английски език",
                        FLHorarium = 108,
                        FLAddition = "(на Английски език - 108 часа)"
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Информационни технологии",
                        GradeText = "Добър",
                        Grade = 3.67m,
                        Horarium = 90
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "История и цивилизации",
                        GradeText = "Много добър",
                        Grade = 4.67m,
                        Horarium = 144
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "География и икономика",
                        GradeText = "Добър",
                        Grade = 3.67m,
                        Horarium = 90
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Философия",
                        GradeText = "Много добър",
                        Grade = 5.33m,
                        Horarium = 162
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Физика и астрономия",
                        GradeText = "Добър",
                        Grade = 4.00m,
                        Horarium = 162
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Химия и опазване на околната среда",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 162
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        DocumentPartId = 14,
                        SubjectId = 45,
                        SubjectName = "Физическо възпитание и спорт",
                        GradeText = "Отличен",
                        Grade = 6.00m,
                        Horarium = 216
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Музика",
                        GradeText = "Отличен",
                        Grade = 5.67m,
                        Horarium = 54
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 2,
                        SubjectName = "Изобразително изкуство",
                        GradeText = "Отличен",
                        Grade = 6.00m,
                        Horarium = 54
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Електротехника",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 90
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Техническа механика",
                        GradeText = "Добър",
                        Grade = 4.00m,
                        Horarium = 18
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Металознание",
                        GradeText = "Добър",
                        Grade = 4.00m,
                        Horarium = 18
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Техническо чертаене",
                        GradeText = "Добър",
                        Grade = 4.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Производствена практика",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 64
                    },
                    //new DiplomaGradeModel()
                    //{
                    //    DocumentPartName = "Избираеми учебни часове",
                    //    SubjectId = 2,
                    //    SubjectName = "Математика ИМ - интегрално смятане при предвиждаен на дърводобива в северната част на южната родопска окръжност",
                    //    GradeText = "Отличен",
                    //    Grade = 5.54m,
                    //    Horarium = 58
                    //},
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Математика ИМ - интегрално смятане при предвиждаен интегрален коефициент",
                        GradeText = "Отличен",
                        Grade = 5.34m,
                        Horarium = 40
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Избираеми учебни часове",
                        SubjectId = 2,
                        SubjectName = "Предприемачество",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 1",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 2",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 3",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 4",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 5",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 6",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 7",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 8",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 9",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 10",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        ExternalEvaluationTypeId = 7,
                        DocumentPartName = "Национално външно оценяване",
                        SubjectId = 1,
                        SubjectName = "Български език и литература",
                        Points = 27
                    },
                    new DiplomaGradeModel()
                    {
                        ExternalEvaluationTypeId = 7,
                        DocumentPartName = "Национално външно оценяване",
                        SubjectId = 2,
                        SubjectName = "Математика",
                        Points = 16
                    },
                    new DiplomaGradeModel()
                    {
                        ExternalEvaluationTypeId = 7,
                        DocumentPartName = "Национално външно оценяване",
                        SubjectId = 28,
                        SubjectName = "Биология и здравно образование",
                        Points = 56
                    },
                    new DiplomaGradeModel()
                    {
                        ExternalEvaluationTypeId = 7,
                        DocumentPartName = "Национално външно оценяване",
                        SubjectId = 100,
                        SubjectName = "Английски език",
                        Points = 56.00m,
                        FLLevel = "B2",
                        SubjectTypeId = 152
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = null,
                        Position = 2,
                        SubjectName = "НОВА РУБРИКА",
                        GradeCategory = -1
                    }
                };
                NonMandatoryGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = null,
                        Position = 1,
                        SubjectName = "НОВА РУБРИКА",
                        GradeCategory = -1
                    },
                    new DiplomaGradeModel()
                    {
                        Position = 2,
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 6 - Много дълъг предмет, който трябва да се раздели на няколко реда при нужда",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        Position = 2,
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 7 - Много дълъг предмет, който трябва да се раздели на няколко реда при нужда",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        Position = 2,
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 8 - Много дълъг предмет, който трябва да се раздели на няколко реда при нужда",
                        GradeText = "Много добър",
                        Grade = 5.00m,
                        Horarium = 36
                    },
                    new DiplomaGradeModel()
                    {
                        Position = 2,
                        DocumentPartName = "Факултативни учебни часове",
                        SubjectId = 2,
                        SubjectName = "ФУЧ 9 - Много дълъг предмет, който трябва да се раздели на няколко реда при нужда",
                        GradeText = "Отличен",
                        Grade = 5.87m,
                        Horarium = 136
                    },
                };
                FLGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 100,
                        SubjectName = "Английски език",
                        GradeText = "Много добър",
                        Grade = 5.34m,
                        Horarium = 180
                    },
                    new DiplomaGradeModel()
                    {
                        DocumentPartName = "Задължителни учебни часове",
                        SubjectId = 101,
                        SubjectName = "Немски език",
                        GradeText = "Добър",
                        Grade = 4.44m,
                        Horarium = 130
                    }
                };
                MandatoryGrades = Grades;
                MandatoryAdditionalGrades = Grades;
            }
        }
        public int BasicDocumentId { get; set; }
        public string BasicDocumentName { get; set; }
        public string Ministry { get; set; }
        public int PersonId { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName ?? ""} {MiddleName ?? ""} {LastName ?? ""}";
            }
        }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegNumberTotal { get; set; }
        public string RegNumberYear { get; set; }
        public string RegDate { get; set; }

        public string Description { get; set; }
        public int SchoolYear { get; set; }
        public int? YearGraduated { get; set; }

        public string Nationality { get; set; }
        public string ProtocolNo { get; set; }
        public string ProtocolDate { get; set; }
        public string EduForm { get; set; }
        public string BasicClassRomeName { get; set; }
        public string BasicClassDescription { get; set; }
        public int? BasicClass { get; set; }
        public decimal? EduDuration { get; set; }
        public int? NKR { get; set; }
        public int? EKR { get; set; }
        public string ITLevel { get; set; }
        public string FLLevel { get; set; }
        public int? VetLevel { get; set; }
        public string VetLevelText { get; set; }
        public string Principal { get; set; }
        public string Deputy { get; set; }
        public string LeadTeacher { get; set; }
        public decimal? Gpa { get; set; }
        public string GpaText { get; set; }
        public string Speciality { get; set; }
        public string SpecialityCode { get; set; }
        public string Profession { get; set; }
        public string ProfessionCode { get; set; }
        public string ClassType { get; set; }
        public int? ClassTypeId { get; set; }
        public int? MandatoryBasicDocumentPartId { get; set; }
        public string GraduationCommissionHead { get; set; }
        public string CommissionOrderNumber { get; set; }
        public string CommissionOrderDate { get; set; }
        public DiplomaPersonModel Person { get; set; }

        /// <summary>
        /// Json данните от Diploma.Content във формат Key(има на поле), Value(стойност на поле)
        /// </summary>
        public Dictionary<string, string> DynamicData { get; set; }

        /// <summary>
        /// Попълване на Зрелостна Комисия
        /// </summary>
        public List<CommissionMember> GraduationCommissionMembers { get; set; }

        public List<DiplomaGradeModel> Grades { get; set; }

        public List<DiplomaGradeModel> FLGrades { get; set; }
        /// <summary>
        /// Оценки от секцията със Задължителни Учебни Часове
        /// </summary>
        public List<DiplomaGradeModel> MandatoryGrades { get; set; }
        /// <summary>
        /// Само допълнителни оценки от секцията със Задължителни Учебни Часове (извън напечатаните)
        /// </summary>
        public List<DiplomaGradeModel> MandatoryAdditionalGrades { get; set; }
        /// <summary>
        /// Оценки от всички секции без Задължителни Учебни Часове
        /// </summary>
        public List<DiplomaGradeModel> NonMandatoryGrades { get; set; }

        /// <summary>
        /// Оценки от диплома, групирани по document.BasicDocumentPart.Code
        /// </summary>
        public Dictionary<string, List<DiplomaGradeModel>> GradesByBasicDocumentPartCode { get; set; }

        /// <summary>
        /// Оценки от диплома, групирани по inst_nom.BasicSubjectType.Abrv и после по SubjectName
        /// </summary>
        public Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> GradesByBasicSubjectTypeAndSubjectName { get; set; }

        /// <summary>
        /// Оценки от външно оценяване, групирани по BasicClass.Name("4", "7", "12", "втора", "трета" и др.)
        /// </summary>
        public Dictionary<string, List<DiplomaGradeModel>> ExtEvalByBasicClass { get; set; }
        public string EducationType { get; set; }
    }

    public class DiplomaPersonModel
    {
        public int Gender { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlaceTown { get; set; }
        public string BirthPlaceMunicipality { get; set; }
        public string BirthPlaceRegion { get; set; }
        public string BirthPlaceTownForeign { get; set; }
        public string BirthPlaceRegionForeign { get; set; }
        public string BirthPlaceMunicipalityForeign { get; set; }
    }

    public class InstitutionModel
    {
        public int? InstitutionId { get; set; }
        public string Institution { get; set; }
        public string FullInstitutionName { get; set; }
        //Тип на населеното място - град = 1, село - 2
        public string InstitutionTownType { get; set; }
        public string InstitutionTown { get; set; }
        public string InstitutionMunicipality { get; set; }
        public string InstitutionLocalArea { get; set; }
        public string InstitutionRegion { get; set; }
        public string OldInstitution { get; set; }
        public string OldInstitutionTown { get; set; }
        public string InstitutionForeign { get; set; }
        public string InstitutionTownForeign { get; set; }
        public string InstitutionMunicipalityForeign { get; set; }
        public string InstitutionRegionForeign { get; set; }
        public string InstitutionLocalAreaForeign { get; set; }
    }

    public class CommissionMember
    {
        public string Name { get; set; }
    }
}
