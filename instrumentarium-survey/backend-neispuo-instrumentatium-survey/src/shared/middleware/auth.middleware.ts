import { Injectable, NestMiddleware } from '@nestjs/common';
import { Request } from 'express';
import jwt from 'jsonwebtoken';
import * as https from 'https';
import * as http from 'http';
import { JwksClient } from 'jwks-rsa';
import { Connection } from 'typeorm';
import { SysRoleEnum } from '../../domain/sys-role/enums/sys-role.enum';
import { AuthObject } from '../interfaces/authed-request.interface';
import { JwtPayloadDTO } from '../services/jwt/jwt.interface';

export interface AuthedRequest extends Request {
    _authObject: AuthObject;
}

/**
 * Checks if request contains a non-expired JWT.
 * If it does, attaches authenticatedUser to the request object, including its roles.
 */
@Injectable()
export class AuthMiddleware implements NestMiddleware {
    jwksClient = new JwksClient({
        jwksUri: process.env.JWKS_ENDPOINT,
        timeout: 60000, // Defaults to 30s,
        requestAgent:
            process.env.ENV != 'dev'
                ? new https.Agent({
                      rejectUnauthorized: false
                  })
                : new http.Agent()
    });

    constructor(private connection: Connection) {}

    async use(request: Request, response: any, next: any) {
        // extract jwt data
        const decodedJwt = await this.getJwtFromHttp(request);
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
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_ADMIN ||
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_EXPERT ||
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM ||
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_OBGUM_FINANCES ||
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_CHRAO ||
                decodedJwt.selected_role.SysRoleID === SysRoleEnum.MON_USER_ADMIN,
            isSchool: decodedJwt.selected_role.SysRoleID === SysRoleEnum.INSTITUTION,
            isNio: decodedJwt.selected_role.SysRoleID === SysRoleEnum.NIO,
            isParent: decodedJwt.selected_role.SysRoleID === SysRoleEnum.PARENT,
            isStudent: decodedJwt.selected_role.SysRoleID === SysRoleEnum.STUDENT,
            isTeacher: decodedJwt.selected_role.SysRoleID === SysRoleEnum.TEACHER
        };
    }

    async getJwtFromHttp(request: Request): Promise<JwtPayloadDTO> {
        let authHeaders = request?.headers?.authorization || request?.headers?.Authorization || '';
        if (authHeaders instanceof Array) {
            authHeaders = authHeaders[0];
        }

        const bearerToken = authHeaders.split(' ')[1]; // 'Bearer jwtTokenString'.split(' ')[1]

        const decoded = await this.verifyToken(bearerToken);
        return decoded;
    }

    // from example: https://www.npmjs.com/package/jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback
    private getKey(header: { kid: string }, callback: (err: Error, signingKey: string) => any) {
        this.jwksClient.getSigningKey(header.kid, (err, key) => {
            if (err) {
                return callback(err, undefined);
            }

            const signingKey = key.getPublicKey();
            callback(null, signingKey);
        });
    }

    private async verifyToken(token: string, options = {}): Promise<JwtPayloadDTO> {
        return new Promise((resolve, reject) => {
            jwt.verify(token, this.getKey.bind(this), options, (err, decoded) => {
                if (err) {
                    return resolve(null);
                }
                resolve(decoded as JwtPayloadDTO);
            });
        });
    }
}
