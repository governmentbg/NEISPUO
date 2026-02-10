import { Request } from 'express';
import { SelectedRole } from '../interfaces/jwt.interface';

export interface AuthObject {
  selectedRole: SelectedRole;
  sessionID?: string;

  // Helper flags derived from selectedRole
  isMonAdmin: boolean;
  isMon: boolean;
  isRuo: boolean;
  isMunicipality: boolean;
  isTeacher: boolean;
  isLeadTeacher: boolean;
  isSchool: boolean;
  isBudgetingInstitution: boolean;
  hasApiKey?: boolean;
}

export interface AuthedRequest extends Request {
  _authObject: AuthObject;
  requestId: string;
}
