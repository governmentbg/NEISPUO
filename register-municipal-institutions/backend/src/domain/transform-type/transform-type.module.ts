import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TransformTypeService } from '../transform-type/routes/transform-type/transform-type.service';
import { TransformTypeController } from '../transform-type/routes/transform-type/transform-type.controller';
import { TransformType } from './transform-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([TransformType])],
    exports: [TypeOrmModule],

    controllers: [TransformTypeController],
    providers: [TransformTypeService],
})
export class TransformTypeModule {}
