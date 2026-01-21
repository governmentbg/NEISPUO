import { Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { AuthSharedGuard } from '@shared/guards/auth-shared-guard';
import {
  AuthedRequest,
  AuthObject,
} from '@shared/interfaces/authed-request.interface';
@Injectable()
export class SubmittedQuestionaireGuard extends AuthSharedGuard {
  
  grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isTeacher, isSchool, isParent, isStudent, isMon, isNio } = authObject;
    
    if (isTeacher || isSchool || isParent || isStudent) {
      this.scopeAccess(authObject, req);
      return true;
    }

    return !!(isMon || isNio);
  }

  grantCreateAccess(authObject: AuthObject) {
    const { isTeacher, isSchool, isParent, isStudent } = authObject;

    return !!(isTeacher || isSchool || isParent || isStudent);
  }

  grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isTeacher, isSchool, isParent, isStudent } = authObject;

    if (isTeacher || isSchool || isParent || isStudent) {
      this.scopeAccess(authObject, req);
      return true;
    }

    return false;
  }

  grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
    return false;
  }

  scopeAccess(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          userId: {
            [CondOperator.EQUALS]: authObject.selectedRole.SysUserID,
          },
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }
}
