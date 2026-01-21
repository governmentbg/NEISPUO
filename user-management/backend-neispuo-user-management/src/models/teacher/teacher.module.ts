import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { StudentTeacherUsersEntity } from '../../common/entities/student-teacher-users-view.entity';
import { InstitutionRepository } from '../institution/institution.repository';
import { TeacherUsersController } from './routing/teacher-users.controller';
import { TeacherUsersService } from './routing/teacher-users.service';
import { TeacherService } from './routing/teacher.service';
import { TeacherRepository } from './teacher.repository';

@Module({
    imports: [TypeOrmModule.forFeature([TeacherRepository, StudentTeacherUsersEntity, InstitutionRepository])],
    providers: [TeacherService, TeacherUsersService],
    exports: [TeacherService, TeacherUsersService],
    controllers: [TeacherUsersController],
})
export class TeacherModule {}
