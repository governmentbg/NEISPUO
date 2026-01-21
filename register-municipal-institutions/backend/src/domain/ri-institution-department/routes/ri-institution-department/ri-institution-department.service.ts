import { Injectable } from '@nestjs/common';
import { RIInstitutionDepartment } from '../../ri-institution-department.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RIInstitutionDepartmentDTO } from './ri-institution-department.dto';

@Injectable()
export class RIInstitutionDepartmentService extends TypeOrmCrudService<RIInstitutionDepartment> {
    constructor(
        @InjectRepository(RIInstitutionDepartment) public repo: Repository<RIInstitutionDepartment>
    ) {
        super(repo);
    }
}
