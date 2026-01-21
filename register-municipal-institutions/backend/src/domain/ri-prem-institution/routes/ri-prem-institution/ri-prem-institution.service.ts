import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RIPremInstitution } from '../../ri-prem-institution.entity';

@Injectable()
export class RIPremInstitutionService extends TypeOrmCrudService<RIPremInstitution> {
    constructor(@InjectRepository(RIPremInstitution) repo: Repository<RIPremInstitution>) {
        super(repo);
    }
}
