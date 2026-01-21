import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { EnrollmentsEntity } from 'src/common/entities/azure-enrollments.entity';

@Injectable()
export class AzureEnrollmentsCrudService extends TypeOrmCrudService<EnrollmentsEntity> {
    constructor(@InjectRepository(EnrollmentsEntity) repo) {
        super(repo);
    }
}
