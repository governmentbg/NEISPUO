import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';

@Injectable()
export class MunicipalityAzureService extends TypeOrmCrudService<ExternalUserViewEntity> {
    constructor(@InjectRepository(ExternalUserViewEntity) repo) {
        super(repo);
    }
}
