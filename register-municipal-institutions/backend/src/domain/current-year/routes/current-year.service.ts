import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { CurrentYearType } from '../current-year.entity';

@Injectable()
export class CurrentYearService extends TypeOrmCrudService<CurrentYearType> {
    constructor(@InjectRepository(CurrentYearType) repo: Repository<CurrentYearType>) {
        super(repo);
    }
}
