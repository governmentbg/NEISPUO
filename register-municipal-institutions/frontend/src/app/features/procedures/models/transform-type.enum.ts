export enum TransformTypeEnum {
  CLOSED = 1, //закриване
  CONVERTED = 2, // преобразувано,
  CHANGED_BUDGETING_INSTITUTION = 2, // промяна на финансиращия орган,
  CREATE = 5, // откриване/вписване,
  CHANGE = 6, // промяна,
  CREATE_CONTINUE = 10, // вписване (продължава дейността),
  JOIN = 11, // преобразуване чрез вливане
  MERGE = 12, // преобразуване чрез сливане
  DETACH = 13, // преобразуване чрез вливане
  DIVIDE = 14, // преобразуване чрез отделяне
  CHANGED_DETACH = 13, // преобразуване чрез отделяне,
  CHANGED_CIRCUMSTANCES = 15, // промяна в обстоятелства,
  CHANGED_BASE_SCHOOL_TYPE = 16, // промяна на вида на институцията,
  CHANGED_ACTIVITY_TYPE = 17, // промяна в предмета на дейност

}
