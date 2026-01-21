import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { SysUser } from '../../sys-user.entity';

@Injectable()
export class SysUserService extends TypeOrmCrudService<SysUser> {
    constructor(@InjectRepository(SysUser) repo: Repository<SysUser>) {
        super(repo);
    }
}
