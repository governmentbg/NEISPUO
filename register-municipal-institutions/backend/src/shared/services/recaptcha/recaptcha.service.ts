import { Injectable, ForbiddenException } from '@nestjs/common';
import * as http from 'http';
import * as https from 'https';
import { ConfigService } from '../config/config.service';

@Injectable()
export class RecaptchaService {
    constructor(private readonly configService: ConfigService) { }

    private getClient(url: string) {
        if (url.indexOf('https') === 0) {
            return https;
        }
        return http;
    }

    async validate(token: string, userIp?: string) {
        const secretKey = process.env.RECAPTCHA_SECRET_KEY;
        let recaptchaApiUrl = 'https://www.google.com/recaptcha/api/siteverify';
        const client = this.getClient(recaptchaApiUrl);
        const validateRecaptcha: {
            success: true | false;
            challenge_ts: number;
            hostname: string;
            'error-codes'?: any[];
        } = await new Promise((resolve, reject) => {
            recaptchaApiUrl += `?secret=${secretKey}&response=${token}`;
            if (userIp) {
                recaptchaApiUrl += `&remoteip=${userIp}`;
            }
            client
                .get(recaptchaApiUrl, (resp) => {
                    let data = '';
                    resp.on('data', (chunk) => {
                        data += chunk;
                    });
                    resp.on('end', () => {
                        let respData;
                        // eslint-disable-next-line prefer-const
                        respData = JSON.parse(data);
                        resolve(respData);
                    });
                })
                .on('error', (err) => {
                    reject(err);
                });
        });

        if (!validateRecaptcha.success) {
            throw new ForbiddenException();
        }
    }
}
