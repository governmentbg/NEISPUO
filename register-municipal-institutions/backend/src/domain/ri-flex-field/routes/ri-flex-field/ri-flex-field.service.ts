import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RIFlexField } from '../../ri-flex-field.entity';

@Injectable()
export class RIFlexFieldService extends TypeOrmCrudService<RIFlexField> {
    constructor(@InjectRepository(RIFlexField) repo: Repository<RIFlexField>) {
        super(repo);
    }
}
