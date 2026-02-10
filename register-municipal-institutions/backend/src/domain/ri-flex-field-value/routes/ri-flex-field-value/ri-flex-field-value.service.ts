import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RIFlexFieldValue } from '../../ri-flex-field-value.entity';

@Injectable()
export class RIFlexFieldValueService extends TypeOrmCrudService<RIFlexFieldValue> {
    constructor(@InjectRepository(RIFlexFieldValue) repo: Repository<RIFlexFieldValue>) {
        super(repo);
    }
}
