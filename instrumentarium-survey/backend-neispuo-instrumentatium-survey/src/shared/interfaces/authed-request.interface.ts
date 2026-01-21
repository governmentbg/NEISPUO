import { Request } from 'express';
import { SysUser } from 'src/domain/sys-user/sys-user.entity';
import { SelectedRole } from '../services/jwt/jwt.interface';

export interface AuthObject {
  user?: SysUser;

  // Helper flags derived from selectedRole
  isMon: boolean;
  isNio: boolean;
  isSchool: boolean;
  isTeacher: boolean;
  isStudent: boolean;
  isParent: boolean;
  selectedRole: SelectedRole;
}

export interface AuthedRequest extends Request {
  _authObject: AuthObject;
}
