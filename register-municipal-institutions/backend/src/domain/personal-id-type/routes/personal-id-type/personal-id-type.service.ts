import { Injectable } from '@nestjs/common';
import { PersonalIDType } from '../../personal-id-type.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class PersonalIDTypeService extends TypeOrmCrudService<PersonalIDType> {
    constructor(@InjectRepository(PersonalIDType) repo: Repository<PersonalIDType>) {
        super(repo);
    }
}
