import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { TransformType } from '../../transform-type.entity';

@Injectable()
export class TransformTypeService extends TypeOrmCrudService<TransformType> {
    constructor(@InjectRepository(TransformType) public repo: Repository<TransformType>) {
        super(repo);
    }
}
