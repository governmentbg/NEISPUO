const SysUserTypesEnum = Object.freeze({
  /** These type of account are related to all the externalRoleEnum roles */
  ADMINISTRATIVE: 0,
  INSTITUTION: 1,
  /** These account types are the ones originated from NEISPUO: Teacher, Students */
  NEISPUO: 2,
  PARENT: 3,
});

module.exports = SysUserTypesEnum;
