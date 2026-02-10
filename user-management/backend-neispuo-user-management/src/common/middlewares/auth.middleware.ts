import { Injectable, Logger, NestMiddleware, UnauthorizedException } from '@nestjs/common';
import { Request } from 'express';
import * as http from 'http';
import * as https from 'https';
import * as jwt from 'jsonwebtoken';
import { JwksClient } from 'jwks-rsa';
import { RoleEnum } from '../constants/enum/role.enum';
import { AuthObject, AuthedRequest } from '../dto/authed-request.interface';
import { JwtPayloadDTO, SelectedRole } from '../dto/jwt.interface';

/**
 * Checks if request contains a non-expired JWT.
 * If it does, attaches authenticatedUser to the request object, including its roles.
 */
@Injectable()
export class AuthMiddleware implements NestMiddleware {
    private readonly logger = new Logger(AuthMiddleware.name);

    jwksClient = new JwksClient({
        jwksUri: process.env.JWKS_ENDPOINT,
        timeout: 60000, // Defaults to 30s,
        requestAgent:
            process.env.APP_ENV !== 'development'
                ? new https.Agent({
                      rejectUnauthorized: false,
                  })
                : new http.Agent(),
    });

    async use(request: Request, response: any, next: any) {
        if (this.requestHasApiKey(request) && this.verifyApiKey(request)) {
            (request as AuthedRequest)._authObject = await this.generateAuthObject(null, request);
        } else {
            try {
                const decodedJwt = await this.getJwtFromHttp(request);
                if (!decodedJwt) {
                    throw new UnauthorizedException();
                }
                (request as AuthedRequest)._authObject = await this.generateAuthObject(decodedJwt, request);
            } catch (e) {
                throw new UnauthorizedException();
            }
        }
        next();
    }

    private async generateAuthObject(decodedJwt: JwtPayloadDTO, request: Request): Promise<AuthObject> {
        const hasVerifiedApiKey = this.requestHasApiKey(request) && this.verifyApiKey(request);
        if (!decodedJwt) {
            return {
                selectedRole: hasVerifiedApiKey ? ({ SysRoleID: RoleEnum.MON_ADMIN } as SelectedRole) : null,
                sessionID: null,
                isMonAdmin: hasVerifiedApiKey,
                isMon: false,
                isRuo: false,
                isSchool: false,
                isTeacher: false,
                isLeadTeacher: false,
                isMunicipality: false,
                isHelpDesk: false,
                hasApiKey: hasVerifiedApiKey,
            };
        }
        return {
            selectedRole: decodedJwt.selected_role,
            sessionID: decodedJwt.sessionID,
            isMonAdmin:
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_ADMIN ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.CIOO,
            isMon:
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_ADMIN ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.CIOO ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_OBGUM ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_OBGUM_FINANCES ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_CHRAO ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.MON_EXPERT,
            isRuo:
                decodedJwt.selected_role.SysRoleID === RoleEnum.RUO ||
                decodedJwt.selected_role.SysRoleID === RoleEnum.RUO_EXPERT,
            isSchool: decodedJwt.selected_role.SysRoleID === RoleEnum.INSTITUTION,
            isTeacher: decodedJwt.selected_role.SysRoleID === RoleEnum.TEACHER,
            isLeadTeacher:
                decodedJwt.selected_role.SysRoleID === RoleEnum.TEACHER &&
                decodedJwt.selected_role.IsLeadTeacher === true,
            isMunicipality: decodedJwt.selected_role.SysRoleID === RoleEnum.MUNICIPALITY,
            isHelpDesk: decodedJwt.selected_role.SysRoleID === RoleEnum.CONSORTIUM_HELPDESK,
            hasApiKey: hasVerifiedApiKey,
        };
    }

    async getJwtFromHttp(request: Request): Promise<JwtPayloadDTO> {
        // eslint-disable-next-line operator-linebreak
        let authHeaders = request?.headers?.authorization || request?.headers?.Authorization || '';
        if (authHeaders instanceof Array) {
            // eslint-disable-next-line prefer-destructuring
            authHeaders = authHeaders[0];
        }

        const bearerToken = authHeaders.split(' ')[1];

        const decoded = (await this.verifyToken(bearerToken)) as Promise<JwtPayloadDTO>;
        return decoded;
    }

    private getKey(header, callback) {
        this.jwksClient.getSigningKey(header.kid, (err, key) => {
            if (err) {
                this.logger.error('Failed to get keys');
                this.logger.error(err);
                callback(err, undefined);
                return;
            }

            try {
                const signingKey = key.getPublicKey();
                callback(null, signingKey);
            } catch (e) {
                callback(e, undefined);
            }
        });
    }

    private async verifyToken(token, options = {}) {
        return new Promise((resolve, reject) => {
            jwt.verify(token, this.getKey.bind(this), options, (err, decoded) => {
                if (err) {
                    this.logger.error('Failed to verify JWT');
                    this.logger.error(err);
                    reject(err);
                }
                resolve(decoded);
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
