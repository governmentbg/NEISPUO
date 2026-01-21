import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { StudentTeacherUsersEntity } from '../../../common/entities/student-teacher-users-view.entity';

@Injectable()
export class StudentUsersService extends TypeOrmCrudService<StudentTeacherUsersEntity> {
    constructor(@InjectRepository(StudentTeacherUsersEntity) repo) {
        super(repo);
    }
}
