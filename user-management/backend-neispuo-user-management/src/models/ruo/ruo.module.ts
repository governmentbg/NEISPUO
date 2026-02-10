import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { RuoAzureController } from './routing/ruo-azure.controller';
import { RuoAzureService } from './routing/ruo-azure.service';
import { RuoService } from './routing/ruo.service';
import { RUORepository } from './ruo.repository';

@Module({
    imports: [TypeOrmModule.forFeature([RUORepository, ExternalUserViewEntity])],
    providers: [RuoService, RuoAzureService],
    controllers: [RuoAzureController],
    exports: [RuoService, RuoAzureService],
})
export class RuoModule {}
