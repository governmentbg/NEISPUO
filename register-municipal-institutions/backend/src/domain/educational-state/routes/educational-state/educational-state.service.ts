import { Injectable } from '@nestjs/common';
import { EducationalState } from '../../educational-state.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class EducationalStateService extends TypeOrmCrudService<EducationalState> {
    constructor(@InjectRepository(EducationalState) repo: Repository<EducationalState>) {
        super(repo);
    }
}
