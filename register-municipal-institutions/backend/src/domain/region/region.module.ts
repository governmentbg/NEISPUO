import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RegionService } from './routes/region/region.service';
import { RegionController } from './routes/region/region.controller';
import { Region } from './region.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Region])],
    exports: [TypeOrmModule],

    controllers: [RegionController],
    providers: [RegionService],
})
export class RegionModule {}
