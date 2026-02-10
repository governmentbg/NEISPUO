/* it is very important that this ENUM, the one from the Front end and the database
 table with roles are at sync at all times */
export enum PositionEnum {
    EMPLOYEE = 2,
    STUDENT_INSTITUTION = 3,
    STUDENT_OTHER_INSTITUTION = 7,
    STUDENT_PLR = 8,
    UNATTENTDING = 9,
    SPECIAL_STUDENT = 10,
}
