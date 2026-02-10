import { Injectable } from '@nestjs/common';
import { SysRole } from '../../sys-role.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class SysRoleService extends TypeOrmCrudService<SysRole> {
    constructor(@InjectRepository(SysRole) repo: Repository<SysRole>) {
        super(repo);
    }
}
