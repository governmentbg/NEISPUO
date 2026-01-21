using MON.Models;
using MON.Models.Enums;
using MON.Shared.Interfaces;
using System;
using System.Linq;

namespace MON.DataAccess
{
    public partial class StudentClass : IAuditable, IInstitutionNotNullable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="classNumber">Номер в клас. Задължително.</param>
        /// <param name="positionId">Позиция(core.Position). Задължително. Основната е 3 - учащ (училище/ДГ)</param>
        /// <param name="basicClassId"></param>
        /// <param name="classTypeId"></param>
        /// <returns></returns>
        public static StudentClass FromModel(StudentClassModel model,
            StudentClassEnrollmentValidationResult validationModel,
            int classNumber, bool withStudentEduForm)
        {
            return new StudentClass
            {
                SchoolYear = model.SchoolYear,
                InstitutionId = validationModel.TargetInstitutionId ?? default,
                ClassId = model.ClassId,
                PersonId = model.PersonId,
                IsIndividualCurriculum = model.HasIndividualStudyPlan ?? false,
                IsHourlyOrganization = model.IsHourlyOrganization ?? false,
                IsForSubmissionToNra = !(model.IsNotForSubmissionToNra ?? false),
                HasSuportiveEnvironment = model.HasSupportiveEnvironment ?? false,
                IsFtacoutsourced = model.IsFTACOutsourced ?? false,
                SupportiveEnvironment = model.SupportiveEnvironment,
                CommuterTypeId = model.CommuterTypeId != 0 ? model.CommuterTypeId : (int?)null,
                RepeaterId = model.RepeaterId != 0 ? model.RepeaterId : -1, // // -1 => Не се посочва,
                StudentEduFormId = withStudentEduForm ? (model.StudentEduFormId ?? -1) : validationModel.TargerClassEduFormId ?? -1, // -1 => не е приложимо
                StudentSpecialityId = model.StudentSpecialityId ?? model.ClassGroup?.ClassSpecialityId ?? -1,
                ClassNumber = classNumber,
                Status = (int)StudentClassStatus.Enrolled,
                IsCurrent = true,
                PositionId = validationModel.TargetPositionId,
                AdmissionDocumentId = model.AdmissionDocumentId,
                BasicClassId = validationModel.TargetBasicClassId ?? model.BasicClassId,
                //validationModel.TargetBasicClassId ?? 21, 21 =>  ДГ разновъзрастова
                ClassTypeId = model.SelectedClassTypeId ?? validationModel.TargetClassTypeId ?? 55, // 55 => Друга
                FromStudentClassId = validationModel.CurrentStudentClassId,
                OrestypeId = model.OresTypeId,
                IsNotPresentForm = validationModel.TargetClassIsNotPresentForm,
                EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date,
                EntryDate = model.EntryDate,
                BuildingAreas = model.BuildingAreas?.Select(x => new BuildingArea
                {
                    BuildingAreaTypeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                AvailableArchitectures = model.AvailableArchitecture?.Select(x => new AvailableArchitecture
                {
                    ModernizationDegreeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                BuildingRooms = model.BuildingRooms?.Select(x => new BuildingRoom
                {
                    BuildingRoomTypeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                SpecialEquipments = model.SpecialEquipment?.Select(x => new SpecialEquipment
                {
                    EquipmentTypeId = x,
                    //TODO - тук трябва да се прави проверка дали институцията има необходимото на ученика обурудване когато таблиците са готови
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList()
            };
        }

        /// <summary>
        /// Използва се когато имаме записване в ЦПЛР.
        /// </summary>
        /// <returns></returns>
        public static StudentClass FromCplrModel(StudentClassModel model,
            EnrollmentTargetClass targetClass, int classNumber)
        {
            // Записване в група в ЦПЛР #1057
            // Правя документ за записване, от стрелката за записване в група отварям initialCplrEnrollment.
            // В StudentClass трябва да се запише Пътуващ null(не е приложимо за ЦПЛР), повторно записан - не се посочва,
            // ОРЕС - null, почасова организация - 0, не се подава в НАП - 1 или null(null е най - вярното).
            // Индивидуален план - 0.

            return new StudentClass
            {
                SchoolYear = model.SchoolYear,
                InstitutionId = targetClass.TargetInstitutionId ?? default,
                ClassId = model.ClassId,
                PersonId = model.PersonId,
                IsIndividualCurriculum = null,
                IsHourlyOrganization = null,
                IsForSubmissionToNra = true,
                HasSuportiveEnvironment = model.HasSupportiveEnvironment ?? false,
                IsFtacoutsourced = model.IsFTACOutsourced ?? false,
                SupportiveEnvironment = model.SupportiveEnvironment,
                CommuterTypeId = null,
                RepeaterId = -1, // Не се посочва
                StudentEduFormId = model.StudentEduFormId ?? targetClass.TargerClassEduFormId ?? -1, // -1 => не е приложимо
                StudentSpecialityId = model.StudentSpecialityId ?? targetClass.TargerClassSpecialityId ?? -1,
                ClassNumber = classNumber,
                Status = (int)StudentClassStatus.Enrolled,
                IsCurrent = true,
                PositionId = targetClass.TargetPositionId,
                AdmissionDocumentId = model.AdmissionDocumentId,
                BasicClassId = targetClass.TargetBasicClassId ?? model.BasicClassId,
                //validationModel.TargetBasicClassId ?? 21, 21 =>  ДГ разновъзрастова
                ClassTypeId = model.SelectedClassTypeId ?? targetClass.TargetClassTypeId ?? 55, // 55 => Друга
                FromStudentClassId = targetClass.CurrentStudentClassId,
                OrestypeId = null,
                IsNotPresentForm = targetClass.TargetClassIsNotPresentForm,
                EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date,
                BuildingAreas = model.BuildingAreas?.Select(x => new BuildingArea
                {
                    BuildingAreaTypeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                AvailableArchitectures = model.AvailableArchitecture?.Select(x => new AvailableArchitecture
                {
                    ModernizationDegreeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                BuildingRooms = model.BuildingRooms?.Select(x => new BuildingRoom
                {
                    BuildingRoomTypeId = x,
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList(),
                SpecialEquipments = model.SpecialEquipment?.Select(x => new SpecialEquipment
                {
                    EquipmentTypeId = x,
                    //TODO - тук трябва да се прави проверка дали институцията има необходимото на ученика обурудване когато таблиците са готови
                    IsAvailable = true,
                    PersonId = model.PersonId
                }).ToList()
            };
        }

        public static StudentClass FromModel(StudentClassBaseModel model,
            StudentClassEnrollmentValidationResult validationModel,
            int classNumber)
        {
            return new StudentClass
            {
                SchoolYear = model.SchoolYear,
                InstitutionId = validationModel.TargetInstitutionId ?? default,
                ClassId = model.ClassId,
                PersonId = model.PersonId,
                RepeaterId = -1, // // -1 => Не се посочва
                StudentEduFormId = validationModel.TargerClassEduFormId ?? -1, // -1 => не е приложимо
                ClassNumber = classNumber,
                Status = (int)StudentClassStatus.Enrolled,
                IsCurrent = true,
                PositionId = validationModel.TargetPositionId,
                BasicClassId = validationModel.TargetBasicClassId ?? 43 , // 43 => Без равнище ,21 =>  ДГ разновъзрастова
                ClassTypeId = model.SelectedClassTypeId ?? validationModel.TargetClassTypeId ?? 55, // 55 => Друга
                FromStudentClassId = validationModel.CurrentStudentClassId,
                IsNotPresentForm = validationModel.TargetClassIsNotPresentForm,
                EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date,
                StudentSpecialityId = validationModel.TargerClassSpecialityId ?? -1 // -1 => не е приложимо
            };
        }

        public void UpdateFrom(StudentClassModel model)
        {
            IsIndividualCurriculum = model.HasIndividualStudyPlan ?? false;
            IsHourlyOrganization = model.IsHourlyOrganization ?? false;
            IsForSubmissionToNra = !(model.IsNotForSubmissionToNra ?? false);
            HasSuportiveEnvironment = model.HasSupportiveEnvironment ?? false;
            SupportiveEnvironment = model.SupportiveEnvironment;
            CommuterTypeId = model.CommuterTypeId != 0 ? model.CommuterTypeId : (int?)null;
            RepeaterId = model.RepeaterId != 0 ? model.RepeaterId : -1; // Не се посочва;
            // Номер в клас вече не се редактира от нашия модул
            // ClassNumber = model.ClassNumber;
            OrestypeId = model.OresTypeId;
            BuildingAreas = model.BuildingAreas?.Select(x => new BuildingArea
            {
                BuildingAreaTypeId = x,
                IsAvailable = true,
                PersonId = model.PersonId
            }).ToList();
            AvailableArchitectures = model.AvailableArchitecture?.Select(x => new AvailableArchitecture
            {
                ModernizationDegreeId = x,
                IsAvailable = true,
                PersonId = model.PersonId
            }).ToList();
            BuildingRooms = model.BuildingRooms?.Select(x => new BuildingRoom
            {
                BuildingRoomTypeId = x,
                IsAvailable = true,
                PersonId = model.PersonId
            }).ToList();
            SpecialEquipments = model.SpecialEquipment?.Select(x => new SpecialEquipment
            {
                EquipmentTypeId = x,
                //TODO - тук трябва да се прави проверка дали институцията има необходимото на ученика обурудване когато таблиците са готови
                IsAvailable = true,
                PersonId = model.PersonId
            }).ToList();


            int basicClassId = model.IsNotPresentForm ? (model.ClassGroup?.BasicClassId ?? model.BasicClassId) : model.BasicClassId;
            if (basicClassId != 0)
            {
                // При 0 каквато е стойността по подразбиране на int ще гръмне базата.
                // Не съществува запис в inst_nom.BasicClass с Id = 0.
                BasicClassId = basicClassId;
            }

            if (model.SelectedClassTypeId.HasValue)
            {
                ClassTypeId = model.SelectedClassTypeId.Value;
            }

            if (model.StudentEduFormId.HasValue)
            {
                StudentEduFormId = model.StudentEduFormId.Value;
            }

            int? specialityId = model.StudentSpecialityId ?? model.ClassGroup?.ClassSpecialityId;
            if (specialityId.HasValue)
            {
                StudentSpecialityId = specialityId.Value;
            }

            if (model.EnrollmentDate.HasValue)
            {
                EnrollmentDate = model.EnrollmentDate.Value.Date;
            }

            EntryDate = model.EntryDate;
        }
    }
}