import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { LocalArea } from '../../local-area.entity';

@Injectable()
export class LocalAreaService extends TypeOrmCrudService<LocalArea> {
    constructor(@InjectRepository(LocalArea) repo: Repository<LocalArea>) {
        super(repo);
    }
}
