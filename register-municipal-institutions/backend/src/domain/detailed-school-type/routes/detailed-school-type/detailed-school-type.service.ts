import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { DetailedSchoolType } from '../../detailed-school-type.entity';

@Injectable()
export class DetailedSchoolTypeService extends TypeOrmCrudService<DetailedSchoolType> {
    constructor(@InjectRepository(DetailedSchoolType) repo: Repository<DetailedSchoolType>) {
        super(repo);
    }
}
