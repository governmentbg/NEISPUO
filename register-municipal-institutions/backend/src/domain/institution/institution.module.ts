import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TownModule } from '@domain/town/town.module';
import { TownService } from '@domain/town/routes/town/town.service';
import { InstitutionService } from './routes/institution/institution.service';
import { InstitutionController } from './routes/institution/institution.controller';
import { Institution } from './institution.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Institution]), TownModule],
    exports: [TypeOrmModule],

    controllers: [InstitutionController],
    providers: [InstitutionService, TownService],
})
export class InstitutionModule {}
