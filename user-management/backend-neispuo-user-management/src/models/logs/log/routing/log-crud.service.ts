import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { LogEntity } from 'src/common/entities/log.entity';

@Injectable()
export class LogCrudService extends TypeOrmCrudService<LogEntity> {
    constructor(@InjectRepository(LogEntity) repo) {
        super(repo);
    }
}
