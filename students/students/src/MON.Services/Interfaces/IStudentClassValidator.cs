using MON.DataAccess;
using MON.Models;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Class;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IStudentClassValidator
    {
        Task<(bool showInitialEntollmentButtonCheck, string showInitialEntollmentButtonCheckError)> ShowInitialEntollmentButton(AdmissionDocumentViewModel admissionDocument);

        Task GetInitialEnrollmentTargetClassDetails(StudentClassModel model);
        Task GetInitialCplrEnrollmentTargetClassDetails(StudentClassModel model);
        Task GetAdditionalEnrollmentTargetClassDetails(StudentClassBaseModel model);
        Task GetCplrAdditionalEnrollmentTargetClassDetail(StudentClassBaseModel model);

        Task<StudentClassEnrollmentValidationResult> ValidateInitialEnrollment(StudentClassModel model);
        Task<StudentClassEnrollmentValidationResult> ValidateInitialCplrEnrollment(StudentClassModel model);
        Task<StudentClassEnrollmentValidationResult> ValidateAdditionalClassEnrollment(StudentClassBaseModel model);
        Task<StudentClassEnrollmentValidationResult> ValidateCplrAdditionalClassEnrollment(StudentClassBaseModel model);

        Task<StudentClassEnrollmentValidationResult> ValidateUpdate(StudentClassBaseModel model, StudentClass entity);

        Task<StudentClassEnrollmentValidationResult> ValidateUnenrollment(StudentClass entity, StudentClassUnenrollmentModel model);

        /// <summary>
        /// Валидация на премстването в паралелка в рамките на една институция.
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща текуща паралелка. Текущата паралелка е задължителна.
        /// 3. Проверка на правата.
        /// 4. Проверка дали текущата паралелка е текуща(IsCurrent == true).
        /// 5. Проверка за невалидна или липсваща паралелка, в която ще преместваме.
        /// 6. Проверка дали паралелката, в която ще преместване има родител (ClassGroup.ParentClassID != null). Задължително записваме само в такива.
        /// 7. Може да се мести от паралелка в паралелка, ако те са един и същи вид - ClassType -- тази проверка не е актуална(М.Митова 15.12.2021)
        /// 8. Проверка дали PersonId-то на тукащата паралека съвпата с подаденото в модела.
        /// 9. Проверка дали институцията на текущата и новата паралелка е една и съща.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentStudentClass"></param>
        /// <returns></returns>\
        Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentClassModel model, StudentClass currentStudentClass);
        Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentAdditionalClassChangeModel model, StudentClass currentStudentClass);
        Task<StudentClassEnrollmentValidationResult> ValidatePositionChange(StudentPositionChangeModel model, StudentClass entity);
        Task<ApiValidationResult> VisibleAddToNewClassBtnCheck(int personId);
    }
}
