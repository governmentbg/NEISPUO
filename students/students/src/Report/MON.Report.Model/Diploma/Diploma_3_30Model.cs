namespace MON.Report.Model.Diploma
{
    using MON.Report.Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_30Model: DiplomaModel
    {
        public Diploma_3_30Model(bool dummy): base(dummy)
        {
            if (dummy)
            {
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
                        Position = 1,
                        SubjectId = 2,
                        SubjectName = "Математика",
                        Grades = new List<DiplomaGradeModel>()
                        {
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.SpecialNeeds,
                                GradeText = "ПИ",
                                Grade = null,
                                BasicClassId = 5
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.SpecialNeeds,
                                GradeText = "СС",
                                Grade = null,
                                BasicClassId = 6
                            },
                            new DiplomaGradeModel()
                            {
                                DocumentPartId = 101,
                                DocumentPartName = "ФАКУЛТАТИВНИ УЧЕБНИ ЧАСОВЕ",
                                GradeCategory = (int)GradeCategoryEnum.SpecialNeeds,
                                GradeText = "СЗ",
                                Grade = null,
                                BasicClassId = 7
                            }
                        }
                    },
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

                NonMandatoryMultiGrades = MultiGrades;
                MandatoryMultiGrades = MultiGrades;
                MandatoryAdditionalMultiGrades = MultiGrades;
            }
        }

        /// <summary>
        /// Изучавани предмети
        /// </summary>
        public List<MultiYearDiplomaGradeModel> MultiGrades { get; set; }
        public List<MultiYearDiplomaGradeModel> MandatoryMultiGrades { get; set; }
        public List<MultiYearDiplomaGradeModel> NonMandatoryMultiGrades { get; set; }
        public List<MultiYearDiplomaGradeModel> MandatoryAdditionalMultiGrades { get; set; }
        /// <summary>
        /// Изучаван задължителен чужд език
        /// </summary>
        public MultiYearDiplomaGradeModel FLMultiGrade { get; set; }
    }


}
