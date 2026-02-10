import {
  Injectable,
  NestMiddleware,
  UnauthorizedException,
} from '@nestjs/common';
import { Request } from 'express';
import { JwksClient } from 'jwks-rsa';
import * as jwt from 'jsonwebtoken';
import * as http from 'http';
import * as https from 'https';
import { AuthObject } from '../interfaces/authed-request.interface';
import { JwtPayloadDTO, SelectedRole } from '../interfaces/jwt.interface';
import { SysRoleEnum } from '../enums/role.enum';

export interface AuthedRequest extends Request {
  _authObject: AuthObject;
}

/**
 * Checks if request contains a non-expired JWT.
 * If it does, attaches authenticatedUser to the request object, including its roles.
 */
@Injectable()
export class AuthMiddleware implements NestMiddleware {
  /**
   *
   */
  constructor() {}
  jwksClient = new JwksClient({
    jwksUri: process.env.JWKS_ENDPOINT,
    timeout: 60000, // Defaults to 30s,
    requestAgent:
      process.env.ENV !== 'dev'
        ? new https.Agent({
            rejectUnauthorized: false,
          })
        : new http.Agent(),
  });

  async use(request: Request, response: any, next: any) {
    const hasVerifiedApiKey =
      this.requestHasApiKey(request) && this.verifyApiKey(request);
    if (hasVerifiedApiKey) {
      (request as AuthedRequest)._authObject = await this.generateAuthObject(
        null,
        request,
        hasVerifiedApiKey,
      );
    } else {
      try {
        const decodedJwt = await this.getJwtFromHttp(request);
        if (!decodedJwt) {
          throw new UnauthorizedException();
        }
        (request as AuthedRequest)._authObject = await this.generateAuthObject(
          decodedJwt,
          request,
        );
      } catch (e) {
        throw new UnauthorizedException(e);
      }
    }
    next();
  }

  private async generateAuthObject(
    decodedJwt: JwtPayloadDTO,
    request,
    hasVerifiedApiKey = false,
  ): Promise<AuthObject> {
    if (!decodedJwt && hasVerifiedApiKey) {
      return {
        selectedRole: hasVerifiedApiKey
          ? ({ SysRoleID: SysRoleEnum.MON_ADMIN } as SelectedRole)
          : null,
        isMonAdmin: hasVerifiedApiKey,
        isMon: false,
        isRuo: false,
        isSchool: false,
        isTeacher: false,
        isLeadTeacher: false,
        isMunicipality: false,
        isBudgetingInstitution: false,
        hasApiKey: hasVerifiedApiKey,
      };
    } else if (!decodedJwt && !hasVerifiedApiKey) {
      return null;
    }
    return {
      selectedRole: decodedJwt.selected_role,
      isMonAdmin: decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_ADMIN,
      isMon:
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_ADMIN ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.CIOO ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_EXPERT ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM_FINANCES ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_CHRAO,
      isRuo:
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.RUO ||
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.RUO_EXPERT,
      isSchool: decodedJwt.selected_role.SysRoleID === SysRoleEnum.INSTITUTION,
      isTeacher: decodedJwt.selected_role.SysRoleID === SysRoleEnum.TEACHER,
      isLeadTeacher:
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.TEACHER &&
        decodedJwt.selected_role.IsLeadTeacher === true,
      isMunicipality:
        decodedJwt.selected_role.SysRoleID === SysRoleEnum.MUNICIPALITY,
      isBudgetingInstitution:
        decodedJwt.selected_role.SysRoleID ===
        SysRoleEnum.BUDGETING_INSTITUTION,
    };
  }

  async getJwtFromHttp(request: Request): Promise<JwtPayloadDTO> {
    // eslint-disable-next-line operator-linebreak
    let authHeaders =
      request?.headers?.authorization || request?.headers?.Authorization || '';
    if (authHeaders instanceof Array) {
      // eslint-disable-next-line prefer-destructuring
      authHeaders = authHeaders[0];
    }

    const bearerToken = authHeaders.split(' ')[1]; // 'Bearer jwtTokenString'.split(' ')[1]

    try {
      const decoded = (await this.verifyToken(
        bearerToken,
      )) as Promise<JwtPayloadDTO>;
      return decoded;
    } catch (e) {
      console.error(e);
      throw new UnauthorizedException();
    }
  }

  // from example: https://www.npmjs.com/package/jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback

  private getKey(header: any, callback: any) {
    this.jwksClient.getSigningKey(header.kid, (err, key) => {
      if (err) {
        return callback(err, undefined);
      }

      const signingKey = key.getPublicKey();
      callback(null, signingKey);
    });
  }

  private async verifyToken(token: any, options = {}) {
    return new Promise((resolve, reject) => {
      jwt.verify(token, this.getKey.bind(this), options, (err, decoded) => {
        if (err) {
          return reject(err);
        }
        return resolve(decoded);
      });
    });
  }

  private requestHasApiKey(request: Request): boolean {
    return !!request.get('X-API-KEY');
  }

  private verifyApiKey(request: Request): boolean {
    return request.get('X-API-KEY') === process.env.INTERNAL_API_KEY;
  }
}
