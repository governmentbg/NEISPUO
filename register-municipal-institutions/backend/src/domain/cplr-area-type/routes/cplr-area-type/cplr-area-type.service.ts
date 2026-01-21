import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { CPLRAreaType } from '../../cplr-area-type.entity';

@Injectable()
export class CPLRAreaTypeService extends TypeOrmCrudService<CPLRAreaType> {
    constructor(@InjectRepository(CPLRAreaType) repo: Repository<CPLRAreaType>) {
        super(repo);
    }
}
