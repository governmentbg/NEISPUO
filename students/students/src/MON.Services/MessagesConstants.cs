namespace MON.Services
{

    public static class Messages
    {
        public const string UnauthorizedMessageError = "Нямате необходимите права за извършване на желаното действие.";
        public const string InvalidInstitutionCodeError = "Невалиден код по НЕИСПУО на институция.";
        public const string InvalidRuoDistrictCodeError = "Невалиден код на област на потребител с роля РУО.";
        public const string InvalidOperation = "Невалидна/непозволена операция.";
        public const string EmptyFileError = "Празен файл.";
        public const string EmptyModelError = "Моделът не може да бъде празен.";
        public const string EmptyEntityError = "Не е намерен запис.";
        public const string ValidationError = "Грешка при валидация.";
        public const string InvalidIdentificatorError = "Невалиден идентификатор.";
        public const string InvalidIdentificationTypeError = "Невалиден вид на идентификатора на детето/ученика.";
        public const string IsFinalizedError = "Записът вече е финализиран.";
        public const string IsSignedError = "Записът вече е подписан.";
        public const string PastSchoolYearError = "Невалиден избор на минала учебна година.";
        public const string SchoolBookMissingDataError = "Липсват данни от дневника.";
        public const string MissingDataError = "Липсват данни.";
        public const string InvalidInstTypeError = "Непозволен вид институция.";
        public const string NotEnrolledInInstitution = "Детето/ученикът не е записан във Вашата институция.";
        public const string EnrolledInHostInstitution = "Не е позволено създаването на документ за отписване за това дете/ученик. Детето/ученикът е записан в паралелка/група на друга институция и продължава обучението си. За да го освободите от Вашата институция, създайте документ за преместване.";
        public const string AbsenceCampaignAlreadyExits = "Вече същестува кампания за отсъствия за избраните месец и година.";
        public const string MissingActiveCampaignForSelectedSchoolYearAndMont = "За избраните учебна година и месец липсва активна кампания за подаване на отсъствия. Редакцията не е позволена.";
        public const string ArgumentError = "Невалидни входни данни";
        public const string DiplomaАlreadyАnulled = "Дипломата вече е анулирана!";
        public const string DiplomaUpdateError = "Дипломата не може да бъде редактирана!";
        public const string AlreadyCompleted = "Вече е маркиран като обработен!";
        public const string RefugeeRequestMissingChildren = "В заявлението липсват въведени деца/уеници!";
        public const string RefugeeRequestChildWithMissingRuoAttrs = "В заявлението има деца/уеници с липсващи 'Номер на заповед на РУО','Дата на заповед на РУО' или 'Институция'!";
        public const string AdmissionPermissionRequestInfo = "Повече информация може да намерите в секция \"Списък с искания за преместване на дете/ученик\" на директорското табло (началната страница на модул \"Деца и ученици\")";
        public const string AlreadyDeleted = "Записът вече е изтрит";
        public const string InvalidSchoolYear = "Институцията не е преминала в нова учебна година!";
        public const string InvalidBasicDocumentRegDate = "Невалиден избор в поле Дата на регистрация! \nДатата на регистрация за избрания вид документ не може да бъде по-ранна от {0}";
        public const string DocManagementCampaignAlreadyExits = "Вече същестува кампания за подаване на заявления за документи с фабрична номерация за избраната учебна година и/или институция и/или период.";
        public const string DocManagementInvalidPeriod = "Невалиден период за заявяване на докuменти с фабрична номерация.";
        public const string DocManagementApplicationAlreadyExits = "Вече същестува заявление за документи с фабрична номерация за избраната кампания.";
        public const string ApplicationStatusError = "Невалиден статус на заявление";

        public static string LodIsFinalizedError(int? schoolYear)
        {
            return schoolYear.HasValue
                ? $"ЛОД е финализирано за учебната година {schoolYear}/{schoolYear+1}."
                : "ЛОД е финализирано за учебната година";
        }

        public static string ExistingStudentClass(int? studentClassId, int? schoolYear, int personId, int classId)
        {
            return $"Въче съществува валиден запис с ID: {studentClassId} за учебна година: {schoolYear}, ученик с ID: {personId} и паралелка с ID: {classId}. Не е позволено запис в една и съща група/паралелка повече от един път.";
        }
    }
}
