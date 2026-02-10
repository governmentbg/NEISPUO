import { Injectable } from '@nestjs/common';
import { SysUserSysRole } from '../../sysuser-sysrole.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class SysUserSysRoleService extends TypeOrmCrudService<SysUserSysRole> {
    constructor(@InjectRepository(SysUserSysRole) repo: Repository<SysUserSysRole>) {
        super(repo);
    }
}
