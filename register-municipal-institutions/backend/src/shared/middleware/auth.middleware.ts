import {
 Inject, Injectable, Logger, NestMiddleware, UnauthorizedException,
} from '@nestjs/common';
import { Request } from 'express';
import { JwksClient } from 'jwks-rsa';
import * as jwt from 'jsonwebtoken';
import * as http from 'http';
import * as https from 'https';
import { AuthObject } from '@shared/interfaces/authed-request.interface';
import { JwtPayloadDTO } from '@shared/services/jwt/jwt.interface';
import { SysRoleEnum } from '@domain/sys-role/enums/sys-role.enum';

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
    constructor(@Inject('winston') private readonly logger: Logger) { }

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
        // extract jwt data
        const decodedJwt = await this.getJwtFromHttp(request);
        // if decodedJwt is empty
        //   req not auth with jwt ->
        //    check for apikey ->
        //      if not -> 401
        if (!decodedJwt) {
            if (!this.verifyApiKey(request)) {
                throw new UnauthorizedException();
            }
        }

        (request as AuthedRequest)._authObject = await this.generateAuthObject(decodedJwt);

        next();
    }

    private async generateAuthObject(decodedJwt: JwtPayloadDTO): Promise<AuthObject> {
        if (!decodedJwt) {
            return null;
        }
        return {
            selectedRole: decodedJwt.selected_role,
            isMon:
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_ADMIN
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.CIOO
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_EXPERT
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM_FINANCES
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_CHRAO,
            isRuo:
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.RUO
                || decodedJwt.selected_role.SysRoleID === SysRoleEnum.RUO_EXPERT,
            isSchool: decodedJwt.selected_role.SysRoleID === SysRoleEnum.INSTITUTION,
            isTeacher: decodedJwt.selected_role.SysRoleID === SysRoleEnum.TEACHER,
            isLeadTeacher:
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.TEACHER
                && decodedJwt.selected_role.IsLeadTeacher === true,
            isMunicipality: decodedJwt.selected_role.SysRoleID === SysRoleEnum.MUNICIPALITY,
        };
    }

    async getJwtFromHttp(request: Request): Promise<JwtPayloadDTO> {
        // eslint-disable-next-line operator-linebreak
        let authHeaders = request?.headers?.authorization || request?.headers?.Authorization || '';
        if (authHeaders instanceof Array) {
            // eslint-disable-next-line prefer-destructuring
            authHeaders = authHeaders[0];
        }

        const bearerToken = authHeaders.split(' ')[1]; // 'Bearer jwtTokenString'.split(' ')[1]

        const decoded = (await this.verifyToken(bearerToken)) as Promise<JwtPayloadDTO>;
        return decoded;
    }

    // from example: https://www.npmjs.com/package/jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback

    private getKey(header: any, callback: any) {
        this.jwksClient.getSigningKey(header.kid, (err, key) => {
            if (err) {
                this.logger.error('Failed to get keys');
                this.logger.error(err);
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
                    this.logger.error('Failed to verify JWT');
                    this.logger.error(err);
                    return resolve(null);
                }
                resolve(decoded);
            });
        });
    }

    private verifyApiKey(request: Request): boolean {
        if (request.get('X-API-KEY') === process.env.INTERNAL_API_KEY) {
            return true;
        }

        this.logger.error('Failed to verify API key');
        return false;
    }
}
