import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { JwksClient } from 'jwks-rsa';
import jwt from 'jsonwebtoken';
import * as http from 'http';
import * as https from 'https';

@Injectable()
export class JwksGuard implements CanActivate {
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

    async canActivate(context: ExecutionContext): Promise<boolean> {
        const request = context.switchToHttp().getRequest();
        const authHeaders = request?.headers?.authorization || request?.headers?.Authorization || '';
        const jwtToken = authHeaders.split(' ')[1]; // 'Bearer jwtTokenString'.split(' ')[1]
        const decoded = await this.verifyToken(jwtToken);
        request.user = decoded;
        return !!decoded;
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
                    return resolve(null);
                }
                resolve(decoded);
            });
        });
    }
}
