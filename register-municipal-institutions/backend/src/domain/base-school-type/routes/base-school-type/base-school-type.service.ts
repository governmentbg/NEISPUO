import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { BaseSchoolType } from '../../base-school-type.entity';

@Injectable()
export class BaseSchoolTypeService extends TypeOrmCrudService<BaseSchoolType> {
    constructor(@InjectRepository(BaseSchoolType) repo: Repository<BaseSchoolType>) {
        super(repo);
    }
}
