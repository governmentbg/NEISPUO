import { Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { AuthSharedGuard } from '@shared/guards/auth-shared-guard';
import {
  AuthedRequest,
  AuthObject,
} from '@shared/interfaces/authed-request.interface';
import { CampaignType } from '../enums/campaign.enum';
@Injectable()
export class CampaignGuard extends AuthSharedGuard {

  grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isSchool, isTeacher, isParent, isStudent, isMon, isNio } = authObject;

    if (isSchool || isTeacher || isParent || isStudent) {
      this.scopeAccess(authObject, req);
      return true;
    }

    if (isMon || isNio) {
      this.scopeESUIAccess(authObject, req);
      return true;
    }
    
    return false;
  }

  grantCreateAccess(authObject: AuthObject) {
    const { isSchool } = authObject;

    return !!isSchool;
  }

  grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isSchool } = authObject;

    if (isSchool) {
      this.scopeAccess(authObject, req);
      return true;
    }

    return false;
  }

  grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isSchool } = authObject;

    if (isSchool) {
      this.scopeAccess(authObject, req);
      return true;
    }

    return false;
  }

  scopeAccess(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          "SubmittedQuestionaires.userId": {
            [CondOperator.EQUALS]: authObject.selectedRole.SysUserID,
          },
          "SubmittedQuestionaires.questionaireId.SysRoleID": {
            [CondOperator.EQUALS]: authObject.selectedRole.SysRoleID,

          }
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  scopeESUIAccess(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          type: {
            [CondOperator.EQUALS]: CampaignType.ESUI,
          },
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }
}
