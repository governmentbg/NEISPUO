namespace MON.Models.Interfaces
{
    using MON.Models.Diploma;
    using MON.Models.Diploma.Import;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Интерфейс за операции, които да се изпълняват при дейности свързани с дипломи
    /// </summary>
    public interface IDiplomaCode
    {
        Task<ApiValidationResult> CanCreate(int basicDocumentId, int personId);

        Task FillGrades(DiplomaCreateModel model);
        Task FillAdditionalDocuments(DiplomaCreateModel model);
        /// <summary>
        /// Дейности след попълване на оценки, като ITLevel и други
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AfterFillGrades(DiplomaCreateModel model);
        void FillExternalEvalGrades(DiplomaCreateModel model);

        /// <summary>
        /// Смята хорариумът на профилиращите предмети, който е сбор от този на модулите.
        /// </summary>
        /// <param name="model"></param>
        void CalcProfSubjectsHorarium(DiplomaCreateModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">document.Diploma.Id</param>
        /// <returns></returns>
        Task AfterFinalization(int id);

        Task<ApiValidationResult> ValidateImportModel(DiplomaImportModel importModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Валидация при подписване на диплома.
        /// Извършваме контрол на рег.номера.
        /// </summary>
        /// <returns></returns>
        Task<ApiValidationResult> ValidateSignModel(DiplomaViewModel diploma);

        Task<ApiValidationResult> ValidateImageDetails(byte[] imageContents, int basicDocumentId, string controlId, string id, int? imagePosition);

        Task<ApiValidationResult> ValidateImageBarcodes(byte[] imageContents, short schoolYear, int basicDocumentId,
            bool hasBarcode, string controlId, string id, int? imagePosition, CancellationToken cancellationToken = default);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="basicDocumentId"></param>
            /// <param name="personId">При създаване на диплома през грида на институцията не може да подадем personId, тъй като данните за ученика се попълват в съдържанието на дипломата.</param>
            /// <param name="contents"></param>
            /// <returns></returns>
        Task<string> AutoFillDynamicContent(int basicDocumentId, int? personId, string contents);
        Task<string> AutoFillDynamicContent(int basicDocumentId, DiplomaImportParseModel diplomaModel, string contents);

        /// <summary>
        /// Попълване на данни за Държавен изпит за проф.квалификация.
        /// </summary>
        /// <param name="basicDocumentId"></param>
        /// <param name="personId"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        Task<string> FillDippkDynamicContent(int basicDocumentId, int? personId, string contents);

        /// <summary>
        /// Попълва някои полета от дипломата (като yearGraduated) в динамичното съдържание
        /// </summary>
        /// <param name="diplomaId"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        Task<string> FixDynamicContent(int diplomaId, string contents);

        void SetBasicClassIds(ICollection<int> ids);
    }
}
