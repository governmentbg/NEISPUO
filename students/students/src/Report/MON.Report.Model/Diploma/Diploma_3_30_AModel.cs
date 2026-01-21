namespace MON.Report.Model.Diploma
{
    using MON.Report.Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_30_AModel: DiplomaModel
    {
        public Diploma_3_30_AModel(bool dummy): base(dummy)
        {
            if (dummy)
            {
                Original = new DiplomaDocumentOriginal()
                {
                    Series = "СС-22",
                    FactoryNumber = "123456",
                    RegNumberTotal = "1824",
                    RegNumberYear = "32",
                    RegDate = "05.09.2022",
                    Institution = "Частна профилирана гимназия с чуждоезиково обучение \"Меридиан 22\"",
                    InstitutionLocalArea = "р-н Младост",
                    InstitutionRegion = "София-град",
                    InstitutionTown = "София",
                    InstitutionMunicipality = "Столична",
                };
                FLMultiGrade = new MultiYearDiplomaGradeModel()
                {
                    DocumentPartId = 101,
                    DocumentPartName = "ЗАДЪЛЖИТЕЛНИ УЧЕБНИ ЧАСОВЕ",
                    Position = 2,
                    SubjectId = 106,
                    SubjectName = "Руски език",
                    Grades = new List<DiplomaGradeModel>()
                        {
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 121,
                                DocumentPartName = "Задължителни учебни часове 5-ти клас",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Много добър",
                                Grade = 6m,
                                BasicClassId = 5
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 122,
                                DocumentPartName = "Задължителни учебни часове 6-ти клас",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Много добър",
                                Grade = 5m,
                                BasicClassId = 6
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 123,
                                DocumentPartName = "Задължителни учебни часове 7-ми клас",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Отличен",
                                Grade = 6m,
                                BasicClassId = 7
                            }
                        }
                }; 

                MultiGrades = new List<MultiYearDiplomaGradeModel>()
                {
                    new MultiYearDiplomaGradeModel()
                    {
                        DocumentPartId = 101,
                        DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                        Position = 10,
                        SubjectId = 101,
                        SubjectName = "Немски език",
                        Grades = new List<DiplomaGradeModel>()
                        {
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Много добър",
                                Grade = 4.50m,
                                BasicClassId = 5
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Много добър",
                                Grade = 5.13m,
                                BasicClassId = 6
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Отличен",
                                Grade = 5.67m,
                                BasicClassId = 7
                            }
                        }
                    },
                    new MultiYearDiplomaGradeModel()
                    {
                        DocumentPartId = 101,
                        DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                        Position = 101,
                        SubjectId = 75,
                        SubjectName = "Икономика",
                        Grades = new List<DiplomaGradeModel>()
                        {
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Среден",
                                Grade = 3m,
                                BasicClassId = 6
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.Normal,
                                GradeText = "Отличен",
                                Grade = 6m,
                                BasicClassId = 7
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Изучавани предмети
        /// </summary>
        public List<MultiYearDiplomaGradeModel> MultiGrades { get; set; }
        public List<MultiYearDiplomaGradeModel> MandatoryMultiGrades { get; set; }
        public List<MultiYearDiplomaGradeModel> NonMandatoryMultiGrades { get; set; }
        /// <summary>
        /// Изучаван задължителен чужд език
        /// </summary>
        public MultiYearDiplomaGradeModel FLMultiGrade { get; set; }

        /// <summary>
        /// Данни за оригинала
        /// </summary>
        public DiplomaDocumentOriginal Original { get; set; }
    }
}
