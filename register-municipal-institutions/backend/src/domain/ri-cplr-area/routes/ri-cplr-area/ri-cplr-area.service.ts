import { Injectable } from '@nestjs/common';
import { RICPLRArea } from '../../ri-cplr-area.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RICPLRAreaDTO } from './ri-cplr-area.dto';

@Injectable()
export class RICPLRAreaService extends TypeOrmCrudService<RICPLRArea> {
    constructor(
        @InjectRepository(RICPLRArea) public repo: Repository<RICPLRArea>
    ) {
        super(repo);
    }
}