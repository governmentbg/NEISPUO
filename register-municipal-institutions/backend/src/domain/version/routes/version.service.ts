import { Injectable } from '@nestjs/common';
import { Version } from '@shared/interfaces/version.interface';
import * as fs from 'fs';

@Injectable()
export class VersionService {
    packageJSON = JSON.parse(fs.readFileSync('./package.json', 'utf8'));

    constructor() { }

    getVersion(): Version {
        return { name: this.packageJSON.name, version: this.packageJSON.version };
    }
}
