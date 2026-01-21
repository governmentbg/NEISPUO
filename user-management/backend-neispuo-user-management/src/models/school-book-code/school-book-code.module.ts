import { Module } from '@nestjs/common';
import { SchoolBookCodeController } from './routing/school-book-code.controller';
import { SchoolBookCodeService } from './routing/school-book-code.service';
import { SchoolBookCodeRepository } from './school-book-code.repository';

@Module({
    controllers: [SchoolBookCodeController],
    providers: [SchoolBookCodeService, SchoolBookCodeRepository],
    exports: [SchoolBookCodeService],
})
export class SchoolBookCodeModule {}
