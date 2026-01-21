import * as fs from 'fs';
import * as jwt from 'jsonwebtoken';
import { Injectable, Global } from '@nestjs/common';
import { ConfigService } from '../config/config.service';
import { JwtPayloadDTO_Deprecated, JwtContent_Deprecated } from './jwt.interface';

@Global()
@Injectable()
export class JwtService {
    private readonly cert = fs.readFileSync(this.configService.get('JWT_CERT_PATH'));

    constructor(private readonly configService: ConfigService) {}

    verify(token: string): Promise<JwtPayloadDTO_Deprecated> {
        return new Promise((resolve, reject) => {
            jwt.verify(token, this.cert, (err, decoded) => {
                if (err) {
                    resolve(null);
                }

                // Usage of any bellow: https://github.com/DefinitelyTyped/DefinitelyTyped/issues/39911
                resolve(decoded as any);
            });
        });
    }

    verifySync(token: string): Promise<JwtPayloadDTO_Deprecated> {
        // Usage of any bellow: https://github.com/DefinitelyTyped/DefinitelyTyped/issues/39911
        return jwt.verify(token, this.cert) as any;
    }

    sign(payload: JwtContent_Deprecated, options: jwt.SignOptions = {}) {
        // TODO: Decouple JWT from config service. Make default expiration configurable or non optional at app usage
        const expiresIn = options.expiresIn || +this.configService.get('JWT_EXPIRATION_SECONDS');
        options = { ...options, expiresIn };

        return new Promise<string>((resolve, reject) => {
            jwt.sign(payload, this.cert, options, (err, token) => {
                if (err) {
                    reject(err);
                }

                resolve(token);
            });
        });
    }

    signSync(payload: JwtContent_Deprecated, options: jwt.SignOptions = {}) {
        const expiresIn = options.expiresIn || +this.configService.get('JWT_EXPIRATION_SECONDS');
        options = { ...options, expiresIn };

        return jwt.sign(payload, this.cert, options);
    }
}
