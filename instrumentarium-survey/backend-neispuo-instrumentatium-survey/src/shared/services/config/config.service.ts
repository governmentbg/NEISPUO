import { Injectable } from '@nestjs/common';
import * as dotenv from 'dotenv';
import * as fs from 'fs';

/**
 * NOTE:
 * This service is only available after app start.
 * Other parts of application may use dotenv directly.
 * Be aware of that if changing/replacing dotenv implementation.
 */
@Injectable()
export class ConfigService {
    private readonly envConfig: { [key: string]: string };

    constructor(filePath: string) {
        this.envConfig = dotenv.parse(fs.readFileSync(filePath));
    }

    get(key: string): string {
        return this.envConfig[key];
    }
}
