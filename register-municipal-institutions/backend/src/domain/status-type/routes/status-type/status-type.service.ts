import { Injectable } from '@nestjs/common';
import { StatusType } from '../../status-type.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class StatusTypeService extends TypeOrmCrudService<StatusType> {
    constructor(@InjectRepository(StatusType) repo: Repository<StatusType>) {
        super(repo);
    }
}
