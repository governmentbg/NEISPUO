import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { ProcedureType } from '../../procedure-type.entity';

@Injectable()
export class ProcedureTypeService extends TypeOrmCrudService<ProcedureType> {
    constructor(@InjectRepository(ProcedureType) repo: Repository<ProcedureType>) {
        super(repo);
    }
}
