import { Module } from '@nestjs/common';
import { PositionService } from './routes/position/position.service';
import { PositionController } from './routes/position/position.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Position } from './position.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Position])],
    exports: [TypeOrmModule],

    controllers: [PositionController],
    providers: [PositionService]
})
export class PositionModule {}
