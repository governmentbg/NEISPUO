import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { LeadTeacherEntity } from '../../../common/entities/lead-teacher.entity';

@Injectable()
export class LeadTeacherService extends TypeOrmCrudService<LeadTeacherEntity> {
    constructor(@InjectRepository(LeadTeacherEntity) repo) {
        super(repo);
    }
}
