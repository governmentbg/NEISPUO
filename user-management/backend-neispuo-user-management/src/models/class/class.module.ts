import { Module } from '@nestjs/common';
import { ClassRepository } from './class.repository';
import { ClassService } from './routing/class.service';

@Module({
    imports: [],
    providers: [ClassService, ClassRepository],
    exports: [ClassService],
    controllers: [],
})
export class ClassModule {}
