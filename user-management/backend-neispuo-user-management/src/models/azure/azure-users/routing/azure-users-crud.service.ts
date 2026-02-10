import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AzureUsersEntity } from 'src/common/entities/azure-users.entity';

@Injectable()
export class AzureUsersCrudService extends TypeOrmCrudService<AzureUsersEntity> {
    constructor(@InjectRepository(AzureUsersEntity) repo) {
        super(repo);
    }
}
