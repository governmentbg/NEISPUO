import { Injectable } from '@nestjs/common';
import { Role } from '../../role.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class RoleService extends TypeOrmCrudService<Role> {
    constructor(@InjectRepository(Role) repo: Repository<Role>) {
        super(repo);
    }
}
