import { Injectable } from '@nestjs/common';
import { VersionDTO } from '../../../common/dto/version.dto';
import fs from 'fs';

@Injectable()
export class VersionService {
    packageJSON = JSON.parse(fs.readFileSync('./package.json', 'utf8'));

    constructor() {}

    getVersion(): VersionDTO {
        const result = { name: this.packageJSON.name, version: this.packageJSON.version };
        return result;
    }
}
