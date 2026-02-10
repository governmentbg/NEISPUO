import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AzureOrganizationsEntity } from 'src/common/entities/azure-organizations.entity';

@Injectable()
export class AzureOrganizationsCrudService extends TypeOrmCrudService<AzureOrganizationsEntity> {
    constructor(@InjectRepository(AzureOrganizationsEntity) repo) {
        super(repo);
    }
}
