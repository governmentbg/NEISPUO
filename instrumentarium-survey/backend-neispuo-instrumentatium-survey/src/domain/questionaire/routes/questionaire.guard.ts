import { Injectable } from '@nestjs/common';
import { AuthSharedGuard } from '@shared/guards/auth-shared-guard';
import {
  AuthedRequest,
  AuthObject,
} from '@shared/interfaces/authed-request.interface';
@Injectable()
export class QuestionaireGuard extends AuthSharedGuard {

  grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
    return true;
  }

  grantCreateAccess(authObject: AuthObject) {
    return false;
  }

  grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
    return false;
  }

  grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
    return false;
  }
}