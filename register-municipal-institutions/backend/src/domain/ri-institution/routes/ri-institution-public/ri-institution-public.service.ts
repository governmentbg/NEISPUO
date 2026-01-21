import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Repository } from 'typeorm';
import { RIInstitutionLatestActive } from '../../ri-institution-latest-active.entity';

@Injectable()
export class RIInstitutionPublicService extends TypeOrmCrudService<RIInstitutionLatestActive> {
    constructor(
        @InjectRepository(RIInstitutionLatestActive)
        public repo: Repository<RIInstitutionLatestActive>,
    ) {
        super(repo);
    }
}
