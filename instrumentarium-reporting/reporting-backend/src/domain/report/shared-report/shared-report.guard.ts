import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import {
  CondOperator,
  RequestQueryBuilder,
  RequestQueryParser,
} from '@nestjsx/crud-request';
import { HttpMethodEnum } from 'src/shared/enums/http-method.enum';
import { SysRoleEnum } from 'src/shared/enums/role.enum';
import {
  AuthedRequest,
  AuthObject,
} from 'src/shared/interfaces/authed-request.interface';

@Injectable()
export class SharedReportGuard implements CanActivate {
  private allowedRoles = [
    SysRoleEnum.MON_ADMIN,
    SysRoleEnum.MON_EXPERT,
    SysRoleEnum.MON_CHRAO,
    SysRoleEnum.MON_OBGUM,
    SysRoleEnum.MON_OBGUM_FINANCES,
    SysRoleEnum.CIOO,
    SysRoleEnum.RUO,
    SysRoleEnum.RUO_EXPERT,
    SysRoleEnum.MUNICIPALITY,
    SysRoleEnum.BUDGETING_INSTITUTION,
    SysRoleEnum.INSTITUTION,
  ];

  canActivate(ctx: ExecutionContext): Promise<boolean> {
    const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
    const authObject: AuthObject = authedRequest._authObject;
    const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

    return this.grantAccess(method, authObject, authedRequest);
  }

  private async grantAccess(
    reqMethod: HttpMethodEnum,
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    let accessGranted: boolean = false;

    if (
      !authObject ||
      !this.allowedRoles.includes(authObject.selectedRole.SysRoleID)
    ) {
      return false;
    }

    if (reqMethod === HttpMethodEnum.GET) {
      accessGranted = this.grantReadAccess(authObject, req);
    } else if (reqMethod === HttpMethodEnum.POST) {
      accessGranted = false;
    } else if (
      reqMethod === HttpMethodEnum.PATCH ||
      reqMethod === HttpMethodEnum.PUT
    ) {
      accessGranted = false;
    } else if (reqMethod === HttpMethodEnum.DELETE) {
      accessGranted = false;
    }

    return accessGranted;
  }

  private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isMon, isRuo, isMunicipality, isSchool, isBudgetingInstitution } =
      authObject;

    if (isMon) {
      this.scopeAccessForMon(authObject, req);
      return true;
    } else if (isRuo) {
      this.scopeAccessForRuo(authObject, req);
      return true;
    } else if (isMunicipality) {
      this.scopeAccessForMunicipality(authObject, req);
      return true;
    } else if (isBudgetingInstitution) {
      this.scopeAccessForBudgetingInstitution(authObject, req);
      return true;
    } else if (isSchool) {
      this.scopeAccessForInstitution(authObject, req);
      return true;
    } else {
      return false;
    }
  }

  private scopeAccessForMon(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where region is same as regionID */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          'CreatedBy.SysUserID': {
            [CondOperator.NOT_EQUALS]: authObject.selectedRole.SysUserID,
          },
        },
        {
          $or: [
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.MON_ADMIN } },
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.MON_CHRAO } },
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.MON_EXPERT } },
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.MON_OBGUM } },
            {
              SharedWith: {
                [CondOperator.CONTAINS]: SysRoleEnum.MON_OBGUM_FINANCES,
              },
            },
            {
              SharedWith: {
                [CondOperator.CONTAINS]: SysRoleEnum.MON_USER_ADMIN,
              },
            },
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.CIOO } },
          ],
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  private scopeAccessForRuo(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where region is same as regionID */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          'CreatedBy.SysUserID': {
            [CondOperator.NOT_EQUALS]: authObject.selectedRole.SysUserID,
          },
        },
        {
          $or: [
            { SharedWith: { [CondOperator.CONTAINS]: SysRoleEnum.RUO } },
            {
              SharedWith: {
                [CondOperator.CONTAINS]: SysRoleEnum.RUO_EXPERT,
              },
            },
          ],
        },
        {
          $or: [
            {
              RegionID: {
                [CondOperator.EQUALS]: authObject.selectedRole.RegionID,
              },
            },
            {
              RegionID: {
                [CondOperator.EQUALS]: null,
              },
            },
          ],
        },

        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  private scopeAccessForMunicipality(
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where municipality is same as municipalityID */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          SharedWith: {
            [CondOperator.CONTAINS]: SysRoleEnum.MUNICIPALITY,
          },

          'CreatedBy.SysUserID': {
            [CondOperator.NOT_EQUALS]: authObject.selectedRole.SysUserID,
          },
          $or: [
            {
              MunicipalityID: {
                [CondOperator.EQUALS]: authObject.selectedRole.MunicipalityID,
              },
            },
            {
              RegionID: {
                [CondOperator.EQUALS]: null,
              },
              MunicipalityID: {
                [CondOperator.EQUALS]: null,
              },
            },
          ],
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  private scopeAccessForInstitution(
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where institution is same as InstitutionID */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          SharedWith: {
            [CondOperator.CONTAINS]: SysRoleEnum.INSTITUTION,
          },
          'CreatedBy.SysUserID': {
            [CondOperator.NOT_EQUALS]: authObject.selectedRole.SysUserID,
          },
          $or: [
            {
              $or: [
                {
                  RegionID: {
                    [CondOperator.EQUALS]: authObject.selectedRole.RegionID,
                  },
                  MunicipalityID: {
                    [CondOperator.EQUALS]:
                      authObject.selectedRole.MunicipalityID,
                  },
                },
              ],
            },
            {
              RegionID: {
                [CondOperator.EQUALS]: null,
              },
              MunicipalityID: {
                [CondOperator.EQUALS]: null,
              },
            },
          ],
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  private scopeAccessForBudgetingInstitution(
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where institution is same as InstitutionID */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          SharedWith: {
            [CondOperator.CONTAINS]: SysRoleEnum.BUDGETING_INSTITUTION,
          },
          'CreatedBy.SysUserID': {
            [CondOperator.NOT_EQUALS]: authObject.selectedRole.SysUserID,
          },
          $or: [
            {
              $or: [
                {
                  RegionID: {
                    [CondOperator.EQUALS]: authObject.selectedRole.RegionID,
                  },
                  MunicipalityID: {
                    [CondOperator.EQUALS]:
                      authObject.selectedRole.MunicipalityID,
                  },
                },
              ],
            },
            {
              RegionID: {
                [CondOperator.EQUALS]: null,
              },
              MunicipalityID: {
                [CondOperator.EQUALS]: null,
              },
            },
          ],
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }
}
