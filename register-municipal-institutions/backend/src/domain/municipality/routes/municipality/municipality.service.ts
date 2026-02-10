import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Municipality } from '../../municipality.entity';

@Injectable()
export class MunicipalityService extends TypeOrmCrudService<Municipality> {
    constructor(@InjectRepository(Municipality) repo: Repository<Municipality>) {
        super(repo);
    }
}
