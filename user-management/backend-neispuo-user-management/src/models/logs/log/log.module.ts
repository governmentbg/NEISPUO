import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { LogEntity } from 'src/common/entities/log.entity';
import { LogRepository } from './log.repository';
import { LogCrudController } from './routing/log-crud.controller';
import { LogCrudService } from './routing/log-crud.service';
import { LogService } from './routing/log.service';

@Module({
    imports: [TypeOrmModule.forFeature([LogEntity])],
    controllers: [LogCrudController],
    providers: [LogService, LogCrudService, LogRepository],
    exports: [LogService, LogCrudService],
})
export class LogModule {}
