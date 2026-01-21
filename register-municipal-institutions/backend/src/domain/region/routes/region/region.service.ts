import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Region } from '../../region.entity';

@Injectable()
export class RegionService extends TypeOrmCrudService<Region> {
    constructor(@InjectRepository(Region) repo: Repository<Region>) {
        super(repo);
    }
}
