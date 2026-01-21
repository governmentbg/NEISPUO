interface ChildSchoolBookCode {
  schoolBookCode: string;
  personalID: string;
}

export interface ParentRegisterDTO {
  firstName: string;

  middleName: string;

  lastName: string;

  password: string;

  email: string;
  childrenCodes: ChildSchoolBookCode[];
}
