import { Request } from 'express';
import { SysUser } from '../../domain/sys-user/sys-user.entity';
import { SelectedRole } from '../services/jwt/jwt.interface';

export interface AuthObject {
    user?: SysUser;
    selectedRole: SelectedRole;

    // Helper flags derived from selectedRole
    isMon: boolean;
    isRuo: boolean;
    isMunicipality: boolean;
    isTeacher: boolean;
    isLeadTeacher: boolean;
    isSchool: boolean;
}

export interface AuthedRequest extends Request {
    _authObject: AuthObject;
}
