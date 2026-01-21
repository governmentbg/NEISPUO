import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { LeadTeacherEntity } from 'src/common/entities/lead-teacher.entity';
import { LeadTeacherController } from './routing/lead-teacher.controller';
import { LeadTeacherService } from './routing/lead-teacher.service';

@Module({
    imports: [TypeOrmModule.forFeature([LeadTeacherEntity])],
    controllers: [LeadTeacherController],
    providers: [LeadTeacherService],
    exports: [LeadTeacherService],
})
export class LeadTeacherModule {}
