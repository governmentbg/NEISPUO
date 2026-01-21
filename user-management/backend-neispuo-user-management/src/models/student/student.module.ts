import { Module, forwardRef } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { StudentUsersController } from './routing/student-users.controller';
import { StudentService } from './routing/student.service';
import { StudentUsersService } from './routing/students-users.service';
import { StudentTeacherUsersEntity } from '../../common/entities/student-teacher-users-view.entity';
import { StudentRepository } from './student.repository';
import { AzureStudentModule } from '../azure/azure-student/azure-student.module';

@Module({
    imports: [TypeOrmModule.forFeature([StudentTeacherUsersEntity]), forwardRef(() => AzureStudentModule)],
    providers: [StudentService, StudentUsersService, StudentRepository],
    exports: [StudentService, StudentUsersService],
    controllers: [StudentUsersController],
})
export class StudentModule {}
