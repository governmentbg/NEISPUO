namespace MON.DataAccess
{
    using Microsoft.EntityFrameworkCore.Internal;
    using MON.Models.StudentModels.Lod;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public partial class LodAssessment : ICreatable, IUpdatable, ILodFinalizationCheckable
    {
        public static LodAssessment FromModel(LodAssessmentCreateModel model, int institutionId)
        {
            LodAssessment subject = new LodAssessment
            {
                PersonId = model.PersonId ?? default,
                SubjectId = model.SubjectId,
                SubjectTypeId = model.SubjectTypeId,
                CurriculumPartId = model.CurriculumPartId ?? default,
                SchoolYear = model.SchoolYear ?? default,
                InstitutionId = institutionId,
                BasicClassId = model.BasicClassId ?? default,
                Position = model.SortOrder,
                Horarium = model.AnnualHorarium,
                FlSubjectId = model.FlSubjectId,
                FlHorarium = model.FlHorarium,
                IsSelfEduForm = model.IsSelfEduForm ?? false,
                IsImported = false,
            };

            if (model.LodAssessmentGrades != null && model.LodAssessmentGrades.Count > 0)
            {
                foreach (LodAssessmentGradeCreateModel gradeModel in model.LodAssessmentGrades)
                {
                    LodAssessmentGrade gradeEntry = LodAssessmentGrade.FromModel(gradeModel);
                    if (gradeEntry != null)
                    {
                        subject.LodAssessmentGrades.Add(gradeEntry);
                    }
                }
            }

            if (model.LodAssessmentChildren != null && model.LodAssessmentChildren.Count > 0)
            {
                foreach (LodAssessmentCreateModel moduleModel in model.LodAssessmentChildren)
                {
                    subject.InverseParent.Add(LodAssessment.FromModel(moduleModel, institutionId));
                }
            }

            return subject;
        }

        public void Update(LodAssessmentCreateModel model, int institutionId)
        {
            if (model == null) return;

            SubjectId = model.SubjectId;
            SubjectTypeId = model.SubjectTypeId;
            Position = model.SortOrder;
            Horarium = model.AnnualHorarium;
            FlSubjectId = model.FlSubjectId;
            FlHorarium = model.FlHorarium;

            ManageGrades(model);
            ManageModules(model, institutionId);
        }

        private void ManageGrades(LodAssessmentCreateModel model)
        {
            foreach (LodAssessmentGradeCreateModel gradeModel in model.LodAssessmentGrades)
            {
                if (gradeModel.GradeId.HasValue)
                {
                    // За добавяне или редакция

                    LodAssessmentGrade gradeEntity = this.LodAssessmentGrades.FirstOrDefault(x => x.Id == gradeModel.Id);
                    if (gradeEntity == null)
                    {
                        // Добавяне на оценка
                        this.LodAssessmentGrades.Add(LodAssessmentGrade.FromModel(gradeModel));
                    } else
                    {
                        // Редакция на оценка
                        gradeEntity.GradeId = gradeModel.GradeId.Value;
                        gradeEntity.DecimalGrade = gradeModel.DecimalGrade;
                    }
                }
                else
                {
                    // За изтриане

                    LodAssessmentGrade gradeToDelete = this.LodAssessmentGrades.FirstOrDefault(x => x.Id == gradeModel.Id);
                    if (gradeToDelete != null)
                    {
                        // Изтриване на оценка
                        this.LodAssessmentGrades.Remove(gradeToDelete);
                    }
                }
            }
        }

        private void ManageModules(LodAssessmentCreateModel model, int institutionId)
        {
            // Модули за изтриване
            List<LodAssessment> modulesToDelete = InverseParent
                .Where(x => !model.LodAssessmentChildren.Any(m => m.Id.HasValue && m.Id == x.Id))
                .ToList();

            foreach (LodAssessment moduleToDelete in modulesToDelete)
            {
                this.InverseParent.Remove(moduleToDelete);
            }


            foreach (LodAssessmentCreateModel moduleCrateModel in model.LodAssessmentChildren)
            {
                LodAssessment moduleEntity = this.InverseParent.FirstOrDefault(x => x.Id == moduleCrateModel.Id);

                if (moduleEntity == null)
                {
                    // Модул за добавяне
                    this.InverseParent.Add(LodAssessment.FromModel(moduleCrateModel, institutionId));
                }
                else
                {
                    // Модул за редакция
                    moduleEntity.Update(moduleCrateModel, institutionId);
                }
            }
        }
    }
}