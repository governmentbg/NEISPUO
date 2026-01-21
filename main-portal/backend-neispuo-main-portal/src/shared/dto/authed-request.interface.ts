import { Request } from 'express';
import { SelectedRole } from './jwt.interface';

export interface AuthObject {
  selected_role: SelectedRole;
}

export interface AuthedRequest extends Request {
  user: AuthObject;
}
