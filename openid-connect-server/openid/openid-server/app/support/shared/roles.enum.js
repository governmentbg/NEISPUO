const externalRolesEnum = Object.freeze({
  MON_ADMINISTRATOR: 1,
  RUO_IO_AI: 2,
  MUNICIPALITY: 3,
  BUDGETING_INSTITUTION: 4,
  /** Parent is created via NEISPUO Telelink API but is not specifically managed in neispuo yet */
  PARENT: 7,
  RUO_EXPERT: 9,
  CIOO: 10,
  EXTERNAL_INSTITUTION_EXPERTS: 11,
  MON_EXPERT: 12,
  MON_OBGUM: 15,
  MON_OBGUM_FINANCE: 16,
  MON_CHRAO: 17,
  NEISPUO_CONSORTIUM_ADMIN: 18,
  NIO: 19,
  ACCOUNTANT: 20,
  EXTERNAL_SUPPLIER: 21,
  TEACHER_HIGHER_EDUCATION: 22,
  PUBLISHER: 23,
});

const internalRolesEnum = Object.freeze({
  INSTITUTION: 0,
  TEACHER: 5,
  STUDENT: 6,
  PARENT: 7,
});

module.exports = { externalRolesEnum, internalRolesEnum };
