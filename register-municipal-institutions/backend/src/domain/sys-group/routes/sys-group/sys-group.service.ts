import { Injectable } from '@nestjs/common';
import { SysGroup } from '../../sys-group.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class SysGroupService extends TypeOrmCrudService<SysGroup> {
    constructor(@InjectRepository(SysGroup) repo: Repository<SysGroup>) {
        super(repo);
    }
}
