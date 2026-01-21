import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { JwksClient } from 'jwks-rsa';
import jwt, { decode } from 'jsonwebtoken';
import https from 'https';
import http from 'http';

@Injectable()
export class JwksGuard implements CanActivate {
  jwksClient = new JwksClient({
    jwksUri: process.env.JWKS_ENDPOINT,
    timeout: 60000, // Defaults to 30s,
    requestAgent:
      process.env.NODE_ENV !== 'development'
        ? new https.Agent({
            rejectUnauthorized: false,
          })
        : new http.Agent(),
  });

  async canActivate(context: ExecutionContext): Promise<boolean> {
    const request = context.switchToHttp().getRequest();
    const authHeaders =
      request?.headers?.authorization || request?.headers?.Authorization || '';
    const jwt = authHeaders.split(' ')[1]; // 'Bearer jwtTokenString'.split(' ')[1]
    const decoded = await this.verifyToken(jwt);
    request.user = decoded;
    return !!decoded;
  }

  // from example: https://www.npmjs.com/package/jsonwebtoken#jwtverifytoken-secretorpublickey-options-callback
  private getKey(header, callback) {
    this.jwksClient.getSigningKey(header.kid, function(err, key) {
      if (err) {
        console.error(`Failed to get keys`);
        console.error(err);
        return callback(err, undefined);
      }

      const signingKey = key.getPublicKey();
      callback(null, signingKey);
    });
  }

  private async verifyToken(token, options = {}) {
    return new Promise((resolve, reject) => {
      jwt.verify(token, this.getKey.bind(this), options, function(
        err,
        decoded,
      ) {
        if (err) {
          console.error(`Failed to verify`);
          console.error(err);
          return resolve(null);
        }
        resolve(decoded);
      });
    });
  }
}
