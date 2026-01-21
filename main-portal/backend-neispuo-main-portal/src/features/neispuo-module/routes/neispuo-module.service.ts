import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { NeispuoModule } from '../neispuo-module.entity';

@Injectable()
export class NesipuoModuleService extends TypeOrmCrudService<NeispuoModule>{
    constructor(@InjectRepository(NeispuoModule) repo) {
        super(repo)
    }
}
