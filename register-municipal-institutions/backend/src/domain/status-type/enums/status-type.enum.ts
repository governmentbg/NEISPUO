export enum StatusTypeEnum {
    OPEN = 1, // Открита
    IN_PROGRESS_WITH_DECISION_FROM_MON__OR_MUNICIPALITY = 2, // В ход, очаква допълнително решение от МОН/ община
    IN_PROCESS_OF_APPEAL_BY_INSTITUTION = 3, // В процес на обжалване от институцията
    COMPLETED_AND_CONFIRMED = 4, // Приключена, потвърдена
    COMPLETED_AND_REJECTED = 5, // Приключена, отказана
    CANCELED = 6, // Анулирана
}
