import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Town } from '../../town.entity';

@Injectable()
export class TownService extends TypeOrmCrudService<Town> {
    constructor(@InjectRepository(Town) public repo: Repository<Town>) {
        super(repo);
    }
}
