import { Request } from 'express';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { SelectedRole } from './jwt.interface';

export interface AuthObject {
    user?: SysUserEntity;

    // Helper flags derived from selectedRole
    isMon: boolean;
    isMonAdmin: boolean;
    isRuo: boolean;
    isMunicipality: boolean;
    isSchool: boolean;
    isTeacher: boolean;
    isLeadTeacher: boolean;
    selectedRole: SelectedRole;
    sessionID: string;
    isHelpDesk: boolean;
    hasApiKey: boolean;
}

export interface AuthedRequest extends Request {
    _authObject: AuthObject;
}
