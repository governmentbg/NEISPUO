import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { MonRepository } from './mon.repository';
import { MonService } from './routing/mon.service';
import { MonCrudService } from './routing/mon-crud.service';
import { MonCrudController } from './routing/mon-crud.controller';

@Module({
    imports: [TypeOrmModule.forFeature([MonRepository, ExternalUserViewEntity])],
    providers: [MonService, MonCrudService],
    exports: [MonService, MonCrudService],
    controllers: [MonCrudController],
})
export class MonModule {}
