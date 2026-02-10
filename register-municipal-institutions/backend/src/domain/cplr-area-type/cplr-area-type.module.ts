import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CPLRAreaTypeService } from './routes/cplr-area-type/cplr-area-type.service';
import { CPLRAreaTypeController } from './routes/cplr-area-type/cplr-area-type.controller';
import { CPLRAreaType } from './cplr-area-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([CPLRAreaType])],
    exports: [TypeOrmModule],

    controllers: [CPLRAreaTypeController],
    providers: [CPLRAreaTypeService],
})
export class CPLRAreaTypeModule {}
