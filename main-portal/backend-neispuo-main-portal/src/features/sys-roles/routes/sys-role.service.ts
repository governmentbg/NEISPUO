import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { SysRole } from 'src/entities/sys-role.entity';
import { Repository } from 'typeorm';

@Injectable()
export class SysRoleService extends TypeOrmCrudService<SysRole> {
  constructor(
    @InjectRepository(SysRole)
    repo: Repository<SysRole>,
  ) {
    super(repo);
  }
}
