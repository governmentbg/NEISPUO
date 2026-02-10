/* it is very important that this ENUM, the one from the Front end and the database
 table with roles are at sync at all times */
export enum SysUserTypeEnum {
    ADMINISTRATIVE = 0,
    INSTITUTION = 1,
    NEISPUO = 2,
    PARENT = 3,
}
