import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { NeispuoCategory } from '../neispuo-category.entity';

@Injectable()
export class NesipuoCategoryService extends TypeOrmCrudService<NeispuoCategory> {
    constructor(@InjectRepository(NeispuoCategory) repo) {
        super(repo)
    }
}
