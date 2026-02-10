import { Module } from '@nestjs/common';
import { VersionController } from './routing/version.controller';
import { VersionService } from './routing/version.service';

@Module({
    imports: [],
    controllers: [VersionController],
    providers: [VersionService],
})
export class VersionModule {}
