import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AzureClassesEntity } from 'src/common/entities/azure-classes.entity';

@Injectable()
export class AzureClassesCrudService extends TypeOrmCrudService<AzureClassesEntity> {
    constructor(@InjectRepository(AzureClassesEntity) repo) {
        super(repo);
    }
}
