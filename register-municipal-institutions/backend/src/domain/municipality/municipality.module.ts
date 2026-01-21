import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { MunicipalityService } from './routes/municipality/municipality.service';
import { MunicipalityController } from './routes/municipality/municipality.controller';
import { Municipality } from './municipality.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Municipality])],
    exports: [TypeOrmModule],

    controllers: [MunicipalityController],
    providers: [MunicipalityService],
})
export class MunicipalityModule {}
