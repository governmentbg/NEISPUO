import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Connection, Repository } from 'typeorm';
import { RIInstitutionLatestActive } from '../../ri-institution-latest-active.entity';

@Injectable()
export class RIInstitutionLatestActiveService extends TypeOrmCrudService<
    RIInstitutionLatestActive
> {
    constructor(
        public connection: Connection,
        @InjectRepository(RIInstitutionLatestActive)
        public repo: Repository<RIInstitutionLatestActive>,
    ) {
        super(repo);
    }
}
