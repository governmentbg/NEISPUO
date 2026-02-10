import { Controller, Get } from '@nestjs/common';
import { Version } from '@shared/interfaces/version.interface';
import { VersionService } from './version.service';

@Controller('v1/version')
export class VersionController {
    constructor(private readonly versionService: VersionService) { }

    @Get()
    getVersion(): Version {
        return this.versionService.getVersion();
    }
}
